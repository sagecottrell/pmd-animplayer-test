using breakout.moves.components;
using Godot;

namespace breakout.moves;

[GlobalClass]
public partial class BaseMove : Node2D
{
    public Node2D? Caster;
    double _time = 0;

    public override void _Process(double delta)
    {
        if (Caster != null)
        {
            if (this.TryGetComponent<CasterPosition>(out var pos) && pos.TrackPosition)
                GlobalPosition = Caster.GlobalPosition;
            if (this.TryGetComponent<CasterRotation>(out var rot) && rot.TrackRotation)
                GlobalRotation = Caster.GlobalRotation;
        }

        _time += delta;
        if (this.TryGetComponent<MoveRangeComponent>(out var range))
        {
            var d = delta / range.Time;
            foreach (var node in range.NodesToAnimate)
            {
                node.Position += new Vector2((float)d * range.Range, 0);
            }

            if (_time >= range.Time)
            {
                OnFinish();
            }
        }
    }

    public void OnFinish()
    {
        GlobalSignals.Instance?.MoveFinish(this);
    }
}
