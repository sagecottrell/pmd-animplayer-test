using Godot;

namespace breakout.components;

/// <summary>
/// Offensive component that deals damage to Hurtboxes on collision.
/// </summary>
[GlobalClass]
public partial class HitboxComponent : BaseComponent
{

    [Export]
    public DamageSource DamageSource;

    public HitboxComponent(DamageSource damageSource) : base()
    {
        DamageSource = damageSource;
    }

}

public interface IHitboxComponentModifier
{
}