using Godot;
using Godot.Collections;

namespace breakout.components.scripts;

[GlobalClass]
public partial class AggroArea : Area2D, INodeComponent
{
    [Export]
    public GroupNames KindsToAggro { get; set; } = GroupNames.Units | GroupNames.Buildings;

    [Signal]
    public delegate void OnEnemyEnterEventHandler(Node2D target);
    [Signal]
    public delegate void OnEnemyExitEventHandler(Node2D target);
    [Signal]
    public delegate void OnFriendlyEnterEventHandler(Node2D target);
    [Signal]
    public delegate void OnFriendlyExitEventHandler(Node2D target);

    public override void _Ready()
    {
        AreaEntered += _on_AreaEntered;
        AreaExited += _on_AreaExited;
    }

    private void _on_AreaExited(Area2D area)
    {
        if (area.GetParent() is not Node2D n2d || GetParent() is not Node2D parent || n2d == parent)
            return;
        if (KindsToAggro.ByStringName().Intersect(n2d.GetGroups()).Count > 0)
        {
            if (n2d.TryGetComponent<TeamComponent>(out var otherTeam) && parent.TryGetComponent<TeamComponent>(out var myTeam) && myTeam == otherTeam)
            {
                EmitSignalOnFriendlyExit(n2d);
                n2d.DisconnectRemovedFromGroupSignal(_onRemoveFriendlyFromGroups);
                return;
            }
            EmitSignalOnEnemyExit(n2d);
            n2d.DisconnectRemovedFromGroupSignal(_onRemoveEnemyFromGroups);
        }
    }

    private void _on_AreaEntered(Area2D area)
    {
        if (area.GetParent() is not Node2D n2d || GetParent() is not Node2D parent || n2d == parent)
            return;
        if (KindsToAggro.ByStringName().Intersect(n2d.GetGroups()).Count > 0)
        {
            if (n2d.TryGetComponent<TeamComponent>(out var otherTeam) && parent.TryGetComponent<TeamComponent>(out var myTeam) && myTeam == otherTeam)
            {
                EmitSignalOnFriendlyEnter(n2d);
                n2d.ConnectRemovedFromGroupSignal(_onRemoveFriendlyFromGroups);
                return;
            }
            EmitSignalOnEnemyEnter(n2d);
            n2d.ConnectRemovedFromGroupSignal(_onRemoveEnemyFromGroups);
        }
        // else: listen to the group join signal so that if they later join a group we care about, we can react to it then.
        // NO: we don't actually need to do this. we can't really get the area from the node
    }

    private void _onRemoveFriendlyFromGroups(Node node, Array<StringName> groups)
    {
        if (node is Node2D n2d && KindsToAggro.ByStringName().Intersect(groups).Count > 0)
        {
            EmitSignalOnFriendlyExit(n2d);
            n2d.DisconnectRemovedFromGroupSignal(_onRemoveFriendlyFromGroups);
        }
    }

    private void _onRemoveEnemyFromGroups(Node node, Array<StringName> groups)
    {
        if (node is Node2D n2d && KindsToAggro.ByStringName().Intersect(groups).Count > 0)
        {
            EmitSignalOnEnemyExit(n2d);
            n2d.DisconnectRemovedFromGroupSignal(_onRemoveEnemyFromGroups);
        }
    }
}
