using breakout.components.AIStrategies.Attacking;
using breakout.components.scripts;
using breakout.customResources;
using Godot;
using System;

namespace breakout.components.AIStrategies;

[Tool]
[GlobalClass]
public partial class DirectAttackStrategy : BaseAttackingStrategy
{
    public override void Attack(Node2D agent, Vector2 target)
    {
        if (agent.TryGetComponent<UnitLevelComponent>(out var level) && agent.TryGetComponent<AttackSpawnComponent>(out var attackSpawn))
        {
            using var cd = RefGetSetMeta.Create(agent, MetadataNames.SquadStrategy.COOLDOWN, 0L);
            if (cd.Value < DateTime.Now.Ticks && level.GetMoveForRange((uint)target.DistanceTo(agent.GlobalPosition)) is MoveDefinition move)
            {
                var s = Math.Max(0.0f, move.BaseCooldownSeconds / level.Speed);
                var newTime = DateTime.Now.AddSeconds(s).Ticks;
                cd.Value = newTime;
                attackSpawn.SpawnAttack(move);
            }
        }
    }
}
