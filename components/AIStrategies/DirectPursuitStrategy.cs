using breakout.components.AIStrategies.TargetChoose;
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
    public BaseTargetChooseStrategy? TargetChooseStrategy { get; set; }
    public Node2D? Target;

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

    override public Vector2 Pathfind(Node2D agent, AIComponent aiStrategy)
    {
        Target ??= TargetChooseStrategy?.GetTarget(agent);
        if (Target is null)
            return Vector2.Zero;
        return Pathfind(Target.GlobalPosition, agent, aiStrategy);
    }

    public override Vector2 Pathfind(Vector2 target, Node2D agent, AIComponent aIComponent)
    {
        var d = target - agent.GlobalPosition;
        if (!_stopped)
        {
            if (d.Length() < StopRadius)
            {
                _stopped = true;
                return agent.GlobalPosition;
            }
            return target;
        }
        if (d.Length() > StartRadius)
        {
            _stopped = false;
            return target;
        }
        return agent.GlobalPosition;
    }
}
