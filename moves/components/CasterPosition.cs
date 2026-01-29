using breakout.components.scripts;
using Godot;

namespace breakout.moves.components;

[GlobalClass]
public partial class CasterPosition : Node, INodeComponent
{
    [Export]
    public bool TrackPosition { get; set; }
}
