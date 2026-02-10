using breakout.components.scripts;
using Godot;

namespace breakout.moves.components;

[Tool]
[GlobalClass]
public partial class MoveRangeComponent : Node, INodeComponent
{
    [Export]
    public uint Range { get; set; }

    [Export]
    public float Time { get; set; } = 1;

    [Export]
    public AnimationPlayer? AnimationPlayer { get; set; }

    public override void _Ready()
    {
        if (AnimationPlayer != null)
        {
            var lib = AnimationPlayer.GetAnimationLibrary(AnimationPlayer.GetAnimationLibraryList()[0]);
            var anim = lib.GetAnimation(lib.GetAnimationList()[0]);
            anim.Length = Time;
            anim.TrackInsertKey(0, Time, Range);
        }
    }
}
