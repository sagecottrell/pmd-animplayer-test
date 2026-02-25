using Godot;

namespace breakout.components.scripts;

/// <summary>
/// Where damage is received.
/// </summary>
[GlobalClass]
public partial class HurtboxComponent : Area2D, INodeComponent
{
    [Signal]
    public delegate void OnHurtEventHandler(DamageSource body);

    [Export]
    public bool DeleteFriendlyFire = true;

    [Export]
    public TeamComponent? Team;

    public void OnHit(HitboxComponent area)
    {
        var node = area.GetParent();
        if (_isFriendly(node))
        {
            if (DeleteFriendlyFire)
                area.DeleteFromFriendlyFire();
            return;
        }
        EmitSignalOnHurt(area.DamageSource);
    }

    private bool _isFriendly(Node node)
    {
        if (GetComponent.TryGetTeamComponent(node, out var otherTeam))
        {
            return Team == otherTeam;
        }
        return false;
    }
}

public interface IHurtboxComponentModifier
{
}