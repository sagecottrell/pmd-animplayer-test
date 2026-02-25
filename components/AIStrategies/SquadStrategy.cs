using breakout.components.AIStrategies.Attacking;
using breakout.components.AIStrategies.TargetChoose;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace breakout.components.AIStrategies;

public enum SquadState
{
    Moving,
    Attacking,
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
            StrategyApproach = StrategyApproach.OnWaveSpawn(),
            StrategyAttack = StrategyAttack.OnWaveSpawn(),
        };
        GlobalSignals.Instance?.SquadCreate(newSquad);
        return newSquad;
    }

    public override Vector2 Pathfind(Node2D agent, AIComponent aiComponent)
    {
        Target ??= TargetChooseStrategy?.GetTarget(agent);
        if (Target is null)
            return agent.GlobalPosition;
        if (AttackingTarget.Count > 0)
            StrategyAttack.Attack(agent, AttackingTarget.FirstOrDefault(Target).GlobalPosition);
        return Pathfind(Target.GlobalPosition, agent, aiComponent);
    }

    public override Vector2 Pathfind(Vector2 target, Node2D agent, AIComponent aiComponent)
    {
        return StrategyApproach?.Pathfind(target, agent, aiComponent) ?? target;
    }

    [Signal]
    public delegate void OnMemberAddedEventHandler(Node2D unit);

    [Signal]
    public delegate void OnMemberRemovedEventHandler(Node2D unit);

    [Export]
    public bool PlayerControllable { get; set; } = false;

    [Export]
    public string SquadGroupName { get; set; } = $"squad-{DateTime.Now.Ticks}";

    [Export]
    public Vector2 FacingDirection { get; set; } = Vector2.Down;

    [Export]
    public BaseTargetChooseStrategy? TargetChooseStrategy { get; set; }
    public Node2D? Target;

    public HashSet<Node2D> AttackingTarget { get; set; } = [];

    public void AddUnit(Node2D unit)
    {
        var existingGroup = unit.GetMeta(MetadataNames.SquadStrategy.GROUP, 0).AsString();
        if (!string.IsNullOrWhiteSpace(existingGroup) && existingGroup != SquadGroupName && unit.TryGetComponent<AIComponent>(out var ai) && ai.Strategy is SquadStrategy ss)
            ss.RemoveUnit(unit);
        unit.AddToGroupsAndSignal(SquadGroupName);
        unit.SetMeta(MetadataNames.SquadStrategy.GROUP, SquadGroupName);
        EmitSignalOnMemberAdded(unit);
    }

    public void RemoveUnit(Node2D unit)
    {
        unit.RemoveMeta(MetadataNames.SquadStrategy.GROUP);
        unit.RemoveFromGroupsAndSignal(SquadGroupName);
        EmitSignalOnMemberRemoved(unit);
    }

    public override void OnEnemyNear(Node2D agent, Node2D enemy)
    {
        AttackingTarget.Add(enemy);
    }

    public override void OnEnemyLeave(Node2D agent, Node2D enemy)
    {
        AttackingTarget.Remove(enemy);
    }

    [Export]
    public AIStrategy StrategyApproach { get; set; } = new DirectPursuitStrategy();
    [Export]
    public BaseAttackingStrategy StrategyAttack { get; set; } = new DirectAttackStrategy();
}
