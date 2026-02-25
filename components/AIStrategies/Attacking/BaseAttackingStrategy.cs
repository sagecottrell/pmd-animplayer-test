using Godot;

namespace breakout.components.AIStrategies.Attacking;

[Tool]
public abstract partial class BaseAttackingStrategy : Resource
{
    public abstract void Attack(Node2D agent, Vector2 target);
    public virtual BaseAttackingStrategy OnComponentReady() => this;
    public virtual BaseAttackingStrategy OnWaveSpawn() => this;
}
