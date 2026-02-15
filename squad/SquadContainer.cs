using breakout.components;
using breakout.components.AIStrategies;
using breakout.components.scripts;
using breakout.customResources;
using Godot;
using Godot.Collections;
using System.Linq;

namespace breakout.squad;

public partial class SquadContainer : Node2D
{
    [Export]
    public SquadFlag? FlagScenePrefab;

    Dictionary<SquadStrategy, SquadFlag> squadNodes = [];
    Node2D? currentSelection;

    public override void _Ready()
    {
        if (FlagScenePrefab is not null)
            FlagScenePrefab.Visible = false;

        foreach (var child in GetChildren())
        {
            if (child is SquadFlag sf && sf.SquadStrategy is not null)
            {
                squadNodes[sf.SquadStrategy] = sf;
                sf.SquadStrategy.Target = sf;
            }
        }
    }

    public override void _Process(double delta)
    {
        // iterate over all squads, trigger AI to tick
        foreach (var squad in squadNodes.Keys)
        {
            foreach (var node in this.SquadMembers(squad))
            {
                if (node.TryGetComponent<AIComponent>(out var ai))
                {
                    ai.Pathfind();
                }
            }
        }
    }

    public void ClearEmptySquads()
    {
        var emptySquads = squadNodes.Where(s => this.SquadMembers(s.Key).Count == 0).ToList();
        foreach (var (squad, node) in emptySquads)
        {
            squadNodes.Remove(squad);
            GlobalSignals.Instance?.SquadDestroy(squad);
            node.QueueFree();
            return;
        }
    }

    public void OnSquadSetPosition(SquadStrategy squadInfo, Vector2 squadGlobalPosition, TeamInfo? team)
    {
        if (squadNodes.TryGetValue(squadInfo, out var squadFlag))
        {
            squadFlag.GlobalPosition = squadGlobalPosition; 
            squadFlag.SetTeam(team);
        }
    }

    public void OnSquadSelected(SquadStrategy? squadInfo)
    { 
        if (currentSelection is not null) 
            currentSelection.Visible = false;
        if (squadInfo is not null && squadNodes.TryGetValue(squadInfo, out var squadFlag))
        {
            currentSelection = squadFlag;
            currentSelection.Visible = true;
        }
        ClearEmptySquads();
    }

    public void OnNewSquad(SquadStrategy squadInfo)
    {
        if (squadNodes.ContainsKey(squadInfo))
            return;
        if (FindUnusedOrNewFlag() is SquadFlag newFlag)
        {
            newFlag.Visible = false;
            squadNodes[squadInfo] = newFlag;
            if (squadInfo.Target is not null)
                newFlag.GlobalPosition = squadInfo.Target.GlobalPosition;
            squadInfo.Target = newFlag;
        }
    }

    public SquadFlag? FindUnusedOrNewFlag()
    {
        foreach (var (squadInfo, flag) in squadNodes)
        {
            if (this.SquadMembers(squadInfo).Count == 0)
            {
                squadNodes.Remove(squadInfo);
                return flag;
            }
        }
        if (FlagScenePrefab?.Duplicate() is SquadFlag newFlag)
        {
            AddChild(newFlag);
            newFlag.Owner = GetOwner();
            return newFlag;
        }
        return null;
    }
}
