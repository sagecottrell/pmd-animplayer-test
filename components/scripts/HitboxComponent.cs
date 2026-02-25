using Godot;
using System.Collections.Generic;

namespace breakout.components.scripts;

/// <summary>
/// Offensive component that deals damage to Hurtboxes on collision.
/// </summary>
[GlobalClass]
public partial class HitboxComponent : Area2D, INodeComponent
{
    [Export]
    public DamageSource? DamageSourceExport;

    public readonly List<Node> Ignore = [];

    bool piercing = false;

    public DamageSource? DamageSourceCode;

    public DamageSource? DamageSource => DamageSourceCode ?? DamageSourceExport;

    public override void _Ready()
    {
        AreaEntered += _hitboxComponent_AreaEntered;
    }

    private void _hitboxComponent_AreaEntered(Area2D area)
    {
        if (area is HurtboxComponent hurtbox && !Ignore.Contains(area.GetParent()))
        {
            hurtbox.OnHit(this);
        }
    }

    public void DeleteFromFriendlyFire()
    {
        if (!piercing)
            GetParent().QueueFree();
    }
}

public interface IHitboxComponentModifier
{
}