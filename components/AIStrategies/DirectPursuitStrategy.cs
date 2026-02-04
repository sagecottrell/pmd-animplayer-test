using Godot;

namespace breakout.components.AIStrategies;

[Tool]
[GlobalClass]
public partial class DirectPursuitStrategy : AIStrategy
{
    private bool _stopped = false;
    private float stopRadius = 20.0f;
    private float startRadius = 25.0f;

    [Export]
    public float StopRadius
    {
        get => stopRadius; set 
        {
            stopRadius = value;
            if (StartRadius < stopRadius)
                StartRadius = stopRadius;
        }
    }

    [Export]
    public float StartRadius
    {
        get => startRadius; set
        {
            startRadius = value;
            if (StopRadius > startRadius)
                StopRadius = startRadius;
        }
    }

    override protected Vector2 Follow(Node2D agent, AIComponent aiStrategy)
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

    protected override Vector2 Attack(Node2D agent, AIComponent aiComponent)
    {
        return Follow(agent, aiComponent);
    }
}
