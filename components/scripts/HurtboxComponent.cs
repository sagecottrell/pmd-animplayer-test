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

    public override void _Ready()
    {
        AreaEntered += OnHit;
    }

    public void OnHit(Area2D area)
    {
        var node = area.GetParent();
        if (GetComponent.TryGetHitboxComponent(node, out var hit))
        {
            if (_isFriendly(node))
            {
                if (DeleteFriendlyFire)
                    hit.DeleteFromFriendlyFire();
                return;
            }
            EmitSignalOnHurt(hit.DamageSource);
        }
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