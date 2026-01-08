using Godot;

namespace breakout.components;

/// <summary>
/// Where damage is received.
/// </summary>
public partial class HurtboxComponent: BaseComponent
{
    [Signal]
    public delegate void OnHurtEventHandler(DamageSource body);

    public void OnHit(Area2D area)
    {
        if (area.GetParent().FindChild(nameof(HitboxComponent)) is HitboxComponent hit)
            EmitSignalOnHurt(hit.DamageSource);
    }
}
