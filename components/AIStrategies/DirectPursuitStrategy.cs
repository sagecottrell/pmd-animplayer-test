using Godot;

namespace breakout.components.AIStrategies;

[GlobalClass]
public partial class DirectPursuitStrategy : AIStrategy
{
    private bool _stopped = false;

    [Export]
    public float StopRadius { get; set; } = 20.0f;

    [Export]
    public float StartRadius { get; set; } = 20.0f;

    override public Vector2 Pathfind(Node2D agent, AIComponent aiStrategy)
    {
        if (aiStrategy.Target is null)
            return Vector2.Zero;
        var d = aiStrategy.Target.GlobalPosition - agent.GlobalPosition;
        if (!_stopped)
        {
            if (d.Length() < StopRadius)
            {
                _stopped = true;
                return Vector2.Zero;
            }
            return d.Normalized();
        }
        if (d.Length() > StartRadius)
        {
            _stopped = false;
            return d.Normalized();
        }
        return Vector2.Zero;
    }
}
