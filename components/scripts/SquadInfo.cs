using Godot;
using Godot.Collections;

namespace breakout.components.scripts;

public enum SquadRank
{
    Frontline = 1,
    Support = 2,
    Rearguard = 3,
}

[Tool]
[GlobalClass]
public partial class SquadInfo : Resource
{
    [Signal]
    public delegate void OnMemberAddedEventHandler(Node2D unit);

    [Signal]
    public delegate void OnMemberRemovedEventHandler(Node2D unit);

    [Export]
    public Godot.Collections.Dictionary<NodePath, bool> Members { get; set; } = [];

    [Export]
    public Dictionary<SquadRank, Array<NodePath>> RanksInSquad { get; set; } = [];

    [Export]
    public Dictionary<NodePath, SquadRank> UnitRanks { get; set; } = [];

    [Export]
    public Vector2 FacingDirection { get; set; } = Vector2.Down;

    public void AddUnit(Node2D unit, SquadRank rank)
    {
#if TOOLS
        GD.Print($"Adding unit {unit.Name} to squad");
#endif
        Members[unit.GetPath()] = true;

        var path = unit.GetPath();
        if (UnitRanks.TryGetValue(path, out var prevRank))
            if (RanksInSquad.TryGetValue(prevRank, out var prevRanks))
                prevRanks.Remove(path);
        UnitRanks[path] = rank;
        if (!RanksInSquad.ContainsKey(rank))
            RanksInSquad[rank] = [];
        if (!RanksInSquad[rank].Contains(path))
            RanksInSquad[rank].Add(path);

        EmitSignalOnMemberAdded(unit);
    }

    public void RemoveUnit(Node2D unit)
    {
#if TOOLS
        GD.Print($"Removing unit {unit.Name} from squad");
#endif
        Members.Remove(unit.GetPath());
        EmitSignalOnMemberRemoved(unit);

        var path = unit.GetPath();
        if (UnitRanks.TryGetValue(path, out var rank))
        {
            UnitRanks.Remove(path);
            if (RanksInSquad.TryGetValue(rank, out var rankUnits))
            {
                rankUnits.Remove(path);
            }
        }
    }
}
