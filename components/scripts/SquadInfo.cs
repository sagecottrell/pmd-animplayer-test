using Godot;
using System.Linq;

namespace breakout.components.scripts;

[Tool]
[GlobalClass]
public partial class SquadInfo : Resource
{
    [Export]
    public Godot.Collections.Dictionary<NodePath, bool> Members { get; set; } = [];

    [Export]
    public Vector2 TargetPosition { get; set; } = Vector2.Zero;

    public void SetTargetPosition(Vector2 position)
    {
        TargetPosition = position;
    }

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

    public Vector2 GetUnitTargetPosition(Node2D unit)
    {
        if (Members is null)
            return unit.GlobalPosition;
        if (Members.Count == 1)
            return TargetPosition;
        int index = Members.Keys.ToList().IndexOf(unit.GetPath());
        if (index == -1)
            return unit.GlobalPosition;
        // Arrange units in a circle around the target position
        float angle = index * (Mathf.Pi * 2 / Members.Count);
        float radius = 50.0f; // Distance from the center
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        return offset + TargetPosition;
    }
}
