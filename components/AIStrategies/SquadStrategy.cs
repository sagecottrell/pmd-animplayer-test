using breakout.components.scripts;
using Godot;
using System.Linq;

namespace breakout.components.AIStrategies;

[GlobalClass]
public partial class SquadStrategy : AIStrategy
{
    [Export]
    public SquadInfo SquadInfo { get; set; } = new();

    [Export]
    public Vector2 FacingDirection { get; set; } = Vector2.Down;

    [Export]
    public int RankInSquad { get; set; } = 0;

    public override Vector2 Pathfind(Node2D agent, AIComponent aiStrategy)
    {
        if (SquadInfo == null || aiStrategy.Target is null)
            return Vector2.Zero;
        Vector2 desiredPosition = GetUnitTargetPosition(agent, aiStrategy.Target);
        Vector2 d = desiredPosition - agent.GlobalPosition;
        if (d.LengthSquared() < 1) {
            if (!aiStrategy.HasReachedTarget)
                aiStrategy.ReachedTarget();
            return Vector2.Zero;
        }
        aiStrategy.HasReachedTarget = false;
        return d.Normalized();
    }

    public Vector2 GetUnitTargetPosition(Node2D unit, Node2D target)
    {
        if (SquadInfo.Members is null)
            return unit.GlobalPosition;
        if (SquadInfo.Members.Count == 1)
            return target.GlobalPosition;
        int index = SquadInfo.Members.Keys.ToList().IndexOf(unit.GetPath());
        if (index == -1)
            return unit.GlobalPosition;
        // Arrange units in a circle around the target position
        float angle = index * (Mathf.Pi * 2 / SquadInfo.Members.Count);
        float radius = 50.0f; // Distance from the center
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        return offset + target.GlobalPosition;
    }
}
