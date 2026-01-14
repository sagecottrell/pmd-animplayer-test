using breakout.components.scripts;
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

    [Export]
    public TeamComponent? Team;

    public void OnHit(Area2D area)
    {
        var node = area.GetParent();
        if (GetComponent.TryGetHitboxComponent(node, out var hit))
        {
            if (_isFriendly(node))
                return;
            EmitSignalOnHurt(hit.DamageSource);
        }
    }

    private bool _isFriendly(Node node)
    {
        if (GetComponent.TryGetTeamComponent(node, out var otherTeam))
        {
            if (Team is not TeamComponent myTeam || otherTeam == null)
                return false;
            return myTeam.Team == otherTeam.Team;
        }
        return false;
    }
}

public interface IHurtboxComponentModifier
{
}