using breakout.components.scripts;
using Godot;

namespace breakout.moves.components;

[GlobalClass]
public partial class CasterRotation : Node, INodeComponent
{
    [Export]
    public bool TrackRotation { get; set; }
}
