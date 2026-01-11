using Godot;

namespace breakout.components.AIStrategies;

[GlobalClass]
public partial class DirectPursuitStrategy : AIStrategy
{
    override public Vector2 Pathfind(Node2D agent, Node2D target)
    {
        return (target.GlobalPosition - agent.GlobalPosition).Normalized();
    }
}
