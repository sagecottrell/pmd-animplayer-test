using breakout.components.scripts;
using Godot;

namespace breakout.moves.components;

[GlobalClass]
public partial class AnimationFinish : AnimationPlayer, INodeComponent
{
    public override void _Ready()
    {
        var lib = (AnimationLibrary)GetAnimationLibrary("").Duplicate();
        RemoveAnimationLibrary("");
        AddAnimationLibrary("", lib);
    }

    public void PlayAnimation()
    {
        Play(GetAnimationList()[0]);
    }
}
