using Godot;

namespace breakout.components.scripts;

/// <summary>
/// Offensive component that deals damage to Hurtboxes on collision.
/// </summary>
[GlobalClass]
public partial class HitboxComponent : Area2D, INodeComponent
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