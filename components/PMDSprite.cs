using Godot;
using System;

namespace breakout.components;

[Tool]
[GlobalClass]
public partial class PMDSprite : Node2D
{
    const string ANIMLIB_NAME = "animationlib";
    private static readonly double THRESHOLD = Math.Sin(Math.PI / 8);

    string facing = "";
    string previous_animation = "";

    AnimationPlayer? Player => GetNode<AnimationPlayer>("AnimationPlayer");

    [Signal]
    public delegate void OnHitEventHandler();

    [Signal]
    public delegate void OnRushEventHandler();

    [Signal]
    public delegate void OnReturnEventHandler();

    [Signal]
    public delegate void OnAnimFinishEventHandler();

    Vector2 dir;
    [Export]
    public Vector2 Direction
    {
        get => dir; set
        {
            dir = value.Normalized();
            UpdateAnimation();
        }
    }


    [Export]
    public AnimationLibrary? Sprites
    {
        get => sprites; set
        {
            sprites = value;
            _ensure_anim_lib(value);
        }
    }
    AnimationLibrary? sprites;

    private void _ensure_anim_lib(AnimationLibrary? lib)
    {
        if (Player is not null)
        {
            if (Player.HasAnimationLibrary(ANIMLIB_NAME))
                Player.RemoveAnimationLibrary(ANIMLIB_NAME);
            if (lib is not null)
                Player.AddAnimationLibrary(ANIMLIB_NAME, lib);
        }
    }

    public override void _Ready()
    {
        _ensure_anim_lib(Sprites);
        Player.AnimationFinished += ResetToPrevious;
    }

    public void ResetToPrevious(StringName? animName = null)
    {
        if (previous_animation != "")
        {
            Player?.Play(previous_animation);
            previous_animation = "";
        }
        EmitSignalOnAnimFinish();
    }

    public void Idle()
    {
        if (Direction.IsZeroApprox())
            Direction = Vector2.Down;
        UpdateAnimation();
    }

    public void Attack()
    {
        if (Player is not null)
        {
            previous_animation = Player.CurrentAnimation;
            Player.Play($"{ANIMLIB_NAME}/Attack-{facing}");
        }
    }

    public void UpdateAnimation()
    {
        if (!IsNodeReady()) return;
        var f = "";
        if (Direction.Y < -THRESHOLD)
            f += "up";
        else if (Direction.Y > THRESHOLD)
            f += "down";
        if (Direction.X < -THRESHOLD)
            f += "left";
        else if (Direction.X > THRESHOLD)
            f += "right";
        if (f != "")
        {
            facing = f;
            Player?.Play($"{ANIMLIB_NAME}/Walk-{f}");
        }
        else
        {
            Player?.Play($"{ANIMLIB_NAME}/Idle-{facing}");
        }
    }

    public void OnHitFrame()
    {
        EmitSignalOnHit();
    }

    public void OnRushFrame()
    {
        EmitSignalOnRush();
    }

    public void OnReturnFrame()
    {
        EmitSignalOnReturn();
    }
}
