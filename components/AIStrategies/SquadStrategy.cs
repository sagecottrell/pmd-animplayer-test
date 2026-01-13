using breakout.components.scripts;
using Godot;

namespace breakout.components.AIStrategies;

[GlobalClass]
public partial class SquadStrategy : AIStrategy
{
    [Export]
    public SquadInfo SquadInfo { get; set; } = new();

    public override Vector2 Pathfind(Node2D agent, AIComponent aiStrategy)
    {
        if (SquadInfo == null)
            return Vector2.Zero;
        Vector2 desiredPosition = SquadInfo.GetUnitTargetPosition(agent);
        Vector2 d = desiredPosition - agent.GlobalPosition;
        if (d.LengthSquared() < 1)
            return Vector2.Zero;
        return d.Normalized();
    }
}
