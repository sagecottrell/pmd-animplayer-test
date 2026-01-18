using Godot;
using System;

namespace breakout.components;

[Tool]
[GlobalClass]
public partial class PMDSprite : Node2D
{
    const string ANIMLIB_NAME = "animationlib";
    private static readonly double THRESHOLD = Math.Sin(Math.PI / 8);

    string? facing;
    string Facing => facing ??= "down";
    string? previous_animation;
    string? current_animation;

    AnimationPlayer? Player => GetNode<AnimationPlayer>("AnimationPlayer");

    [Signal]
    public delegate void OnHitEventHandler();

    [Signal]
    public delegate void OnRushEventHandler();

    [Signal]
    public delegate void OnReturnEventHandler();

    [Signal]
    public delegate void OnAnimFinishEventHandler();

    Vector2 dir = Vector2.Down;
    [Export]
    public Vector2 Direction
    {
        get => dir; set
        {
            if (value.LengthSquared() == 0)
                return;
            dir = value.Normalized();
            UpdateDirection();
        }
    }


    [Export]
    public AnimationLibrary? Sprites
    {
        get => sprites; set
        {
            if (value is null) return;
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
        Player!.AnimationFinished += ResetToPrevious;
    }

    public void ResetToPrevious(StringName? animName = null)
    {
        if (!string.IsNullOrWhiteSpace(previous_animation))
        {
            Player?.Play(previous_animation);
            previous_animation = "";
        }
        EmitSignalOnAnimFinish();
    }

    public void Idle() => Play("Idle");
    public void Walk() => Play("Walk");
    public void Hurt() => Play("Hurt");
    public void Attack() => Play("Attack", "Strike", "Hit");
    public void Shoot() => Play("Shoot");
    public void Charge() => Play("Charge");
    public void Sleep() => Play("Sleep");
    public void Swing() => Play("Swing");
    public void Rotate() => Play("Rotate");
    public void Hop() => Play("Hop");
    public void Double() => Play("Double");
    public void Twirl() => Play("Twirl", "Rotate");
    public void Kick() => Play("Kick", "Attack", "Strike", "Hit");
    public void Appeal() => Play("Appeal", "Hop");
    public void RearUp() => Play("RearUp", "Appeal", "Charge");
    public void SpAttack() => Play("SpAttack", "RearUp", "Attack", "Strike", "Hit");

    public void Play(params string[] animations)
    {
        if (Player is not null)
        {
            foreach (var anim in animations)
            {
                var name = $"{ANIMLIB_NAME}/{anim}-{Facing}";
                if (Player.HasAnimation(name))
                {
                    if (Player.CurrentAnimation == name)
                        break;
                    previous_animation = Player.CurrentAnimation;
                    current_animation = anim;
                    Player.Play(name);
                    return;
                }
            }
        }
    }

    public void UpdateDirection()
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
        if (!string.IsNullOrWhiteSpace(f))
        {
            facing = f;
            Play(current_animation ?? "Idle");
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


public interface IPMDSpriteModifier
{
}