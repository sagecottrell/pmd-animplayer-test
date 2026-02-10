using Godot;

namespace breakout.components.scripts;

[GlobalClass]
public partial class SpeedComponent : Node, INodeComponent
{
    [Export]
    public float Speed { get; set; }
    [Export]
    public Vector2 Velocity { get; set; }
}

public interface ISpeedComponentModifier
{

}