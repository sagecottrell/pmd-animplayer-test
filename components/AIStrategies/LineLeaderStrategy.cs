using Godot;
using System.Collections.Generic;

namespace breakout.components.AIStrategies;

[GlobalClass]
public partial class LineLeaderStrategy : AIStrategy
{
    public override bool CreateCopyOnComponentReady => true;

    [Export]
    public float Radius;
    Queue<Vector2> positions = [];
    Vector2 _last_target_pos;

    public override Vector2 Pathfind(Node2D agent, AIComponent aiStrategy)
    {
        var target = aiStrategy.Target;
        if (target is null)
            return Vector2.Zero;

        var tp = target.GlobalPosition;
        var ap = agent.GlobalPosition;

        if (_last_target_pos.IsEqualApprox(tp) == false)
        {
            _last_target_pos = tp;
            positions.Enqueue(tp);
        }

        Vector2 last = _last_target_pos;
        while (positions.TryPeek(out var peek) && peek.DistanceTo(tp) > Radius)
        {
            last = positions.Dequeue();
        }

        if (tp.DistanceTo(ap) < Radius)
        {
            return Vector2.Zero;
        }

        var d = last - ap;
        return d.Normalized();
    }
}
