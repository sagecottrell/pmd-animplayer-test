using Godot;

namespace breakout.components;

/// <summary>
/// Where damage is received.
/// </summary>
[GlobalClass]
public partial class HurtboxComponent : Node
{
    [Signal]
    public delegate void OnHurtEventHandler(DamageSource body);

    public void OnHit(Area2D area)
    {
        var node = area.GetParent();
        if (node.FindChild(nameof(HitboxComponent)) is HitboxComponent hit)
        {
            if (_isFriendly(node))
                return;
            EmitSignalOnHurt(hit.DamageSource);
        }
    }

    private bool _isFriendly(Node node)
    {
        if (node.FindChild(nameof(TeamComponent)) is TeamComponent otherTeam)
        {
            if (GetParent().FindChild(nameof(TeamComponent)) is not TeamComponent myTeam || otherTeam == null)
                return false;
            return myTeam.Team == otherTeam.Team;
        }
        return false;
    }
}

public interface IHurtboxComponentModifier
{
}