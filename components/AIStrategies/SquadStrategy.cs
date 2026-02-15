using breakout.components.AIStrategies.TargetChoose;
using Godot;
using System;

namespace breakout.components.AIStrategies;

public enum SquadState
{
    Moving,
    Attacking,
    Idle,
}

[Tool]
[GlobalClass]
public partial class SquadStrategy : AIStrategy
{
    public override AIStrategy OnWaveSpawn()
    {
        var newSquad = new SquadStrategy()
        {
            TargetChooseStrategy = TargetChooseStrategy,
        };
        GlobalSignals.Instance?.SquadCreate(newSquad);
        return newSquad;
    }

    public override Vector2 Pathfind(Node2D agent, AIComponent aiStrategy)
    {
        Target ??= TargetChooseStrategy?.GetTarget(agent);
        if (Target is null)
            return agent.GlobalPosition;
        return Pathfind(Target.GlobalPosition, agent, aiStrategy);
    }

    public override Vector2 Pathfind(Vector2 target, Node2D agent, AIComponent aIComponent)
    {
        if (State == SquadState.Moving)
            return Approach?.Pathfind(target, agent, aIComponent) ?? target;
        return target;
    }

    [Signal]
    public delegate void OnMemberAddedEventHandler(Node2D unit);

    [Signal]
    public delegate void OnMemberRemovedEventHandler(Node2D unit);

    [Export]
    public bool PlayerControllable;

    [Export]
    public string SquadGroupName { get; set; } = $"squad-{DateTime.Now.Ticks}";

    [Export]
    public Vector2 FacingDirection { get; set; } = Vector2.Down;

    [Export]
    public BaseTargetChooseStrategy? TargetChooseStrategy { get; set; }
    public Node2D? Target;

    [Export]
    public SquadState State { get; set; } = SquadState.Idle;

    public void AddUnit(Node2D unit)
    {
        var existingGroup = unit.GetMeta(MetadataNames.SquadStrategy.GROUP, 0).AsString();
        if (!string.IsNullOrWhiteSpace(existingGroup) && existingGroup != SquadGroupName && unit.TryGetComponent<AIComponent>(out var ai) && ai.Strategy is SquadStrategy ss)
            ss.RemoveUnit(unit);
        unit.AddToGroup(SquadGroupName);
        unit.SetMeta(MetadataNames.SquadStrategy.GROUP, SquadGroupName);
        EmitSignalOnMemberAdded(unit);
    }

    public void RemoveUnit(Node2D unit)
    {
        unit.RemoveMeta(MetadataNames.SquadStrategy.GROUP);
        unit.RemoveFromGroup(SquadGroupName);
        EmitSignalOnMemberRemoved(unit);
    }

    public override void OnEnemyNear(Node2D agent, Node2D enemy)
    {
    }

    [Export]
    public AIStrategy? Approach { get; set; }
}
