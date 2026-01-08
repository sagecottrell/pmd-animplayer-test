using Godot;

namespace breakout.components;

/// <summary>
/// Offensive component that deals damage to Hurtboxes on collision.
/// </summary>
public partial class HitboxComponent: BaseComponent
{

    [Export]
    public DamageSource DamageSource;

}
