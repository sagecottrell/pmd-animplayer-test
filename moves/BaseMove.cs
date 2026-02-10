using breakout.moves.components;
using Godot;

namespace breakout.moves;

[GlobalClass]
public partial class BaseMove : Node2D
{
    public Node2D? Caster;

    public override void _Process(double delta)
    {
        if (Caster != null)
        {
            if (this.TryGetComponent<CasterPosition>(out var pos) && pos.TrackPosition)
                GlobalPosition = Caster.GlobalPosition;
            if (this.TryGetComponent<CasterRotation>(out var rot) && rot.TrackRotation)
                GlobalRotation = Caster.GlobalRotation;
        }
    }
}
