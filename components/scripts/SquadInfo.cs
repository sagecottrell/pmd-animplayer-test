using Godot;

namespace breakout.components.scripts;

[Tool]
[GlobalClass]
public partial class SquadInfo : Resource
{
    [Export]
    public Godot.Collections.Dictionary<NodePath, bool> Members { get; set; } = [];

    public void AddUnit(Node2D unit)
    {
#if TOOLS
        GD.Print($"Adding unit {unit.Name} to squad");
#endif
        Members[unit.GetPath()] = true;
    }

    public void RemoveUnit(Node2D unit)
    {
#if TOOLS
        GD.Print($"Removing unit {unit.Name} from squad");
#endif
        Members.Remove(unit.GetPath());
    }
}
