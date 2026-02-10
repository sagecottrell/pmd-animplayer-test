using breakout.components.scripts;
using Godot;

namespace breakout.moves.components;

[GlobalClass]
public partial class AnimationFinish : AnimationPlayer, INodeComponent
{
    public void OnFinish(StringName _anim)
    {
        GlobalSignals.Instance?.MoveFinish(GetParent<Node2D>());
    }

    public override void _EnterTree()
    {
        Play(GetAnimationList()[0]);
    }
}
