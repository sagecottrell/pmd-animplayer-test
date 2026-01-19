using Godot;

namespace breakout.components;

/// <summary>
/// Offensive component that deals damage to Hurtboxes on collision.
/// </summary>
[GlobalClass]
public partial class HitboxComponent : Node
{

    [Export]
    public DamageSource? DamageSource;

    bool piercing = false;

    public void DeleteFromFriendlyFire()
    {
        if (!piercing)
            GetParent().QueueFree();
    }
}

public interface IHitboxComponentModifier
{
}