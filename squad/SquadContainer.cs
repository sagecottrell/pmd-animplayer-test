using breakout.components.scripts;
using breakout.resourceTypes;
using Godot;
using Godot.Collections;
using System.Linq;

namespace breakout.squad;

public partial class SquadContainer : Node2D
{
    [Export]
    public SquadFlag? FlagScenePrefab;

    Dictionary<SquadInfo, SquadFlag> squadNodes = [];
    Node2D? currentSelection;

    public override void _Ready()
    {
        if (FlagScenePrefab is not null)
            FlagScenePrefab.Visible = false;
    }

    public void ClearEmptySquads()
    {
        var emptySquads = squadNodes.Where(s => s.Key.Members.Count == 0).ToList();
        foreach (var (squad, node) in emptySquads)
        {
            squadNodes.Remove(squad);
            node.QueueFree();
            return;
        }
    }

    public void OnSquadSetPosition(SquadInfo squadInfo, Vector2 squadGlobalPosition, TeamInfo team)
    {
        if (squadNodes.TryGetValue(squadInfo, out var squadFlag))
        {
            squadFlag.GlobalPosition = squadGlobalPosition;
        }
        else if (FindUnusedOrNewFlag() is SquadFlag newFlag)
        {
            newFlag.GlobalPosition = squadGlobalPosition;
            newFlag.Visible = false;
            newFlag.SetTeam(team);
            squadNodes[squadInfo] = newFlag;

            foreach (var node in squadInfo.Members.Keys.Select(GetTree().Root.GetNode))
            {
                if (GetComponent.TryGetAIComponent(node, out var ai))
                {
                    ai.Target = newFlag;
                }
            }
        }
    }

    public void OnSquadSelected(SquadInfo? squadInfo)
    {
        if (currentSelection is not null) currentSelection.Visible = false;
        if (squadInfo is not null && squadNodes.TryGetValue(squadInfo, out var squadFlag))
        {
            currentSelection = squadFlag;
            currentSelection.Visible = true;
        }
        ClearEmptySquads();
    }

    public SquadFlag? FindUnusedOrNewFlag()
    {
        foreach (var (squad, info) in squadNodes)
        {
            if (squad.Members.Count == 0)
            {
                squadNodes.Remove(squad);
                return info;
            }
        }
        if (FlagScenePrefab?.Duplicate() is SquadFlag newFlag)
        {
            AddChild(newFlag);
            return newFlag;
        }
        return null;
    }
}
