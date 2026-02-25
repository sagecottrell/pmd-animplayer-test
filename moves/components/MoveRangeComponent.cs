using breakout.components.scripts;
using Godot;
using Godot.Collections;

namespace breakout.moves.components;

[GlobalClass]
public partial class MoveRangeComponent : Node, INodeComponent
{
    public uint Range { get; set; }
    public float Time { get; set; }

    [Export]
    public Array<Node2D> NodesToAnimate { get; set; } = [];
}
