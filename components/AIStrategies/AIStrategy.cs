using Godot;

namespace breakout.components.AIStrategies;

[Tool]
[GlobalClass]
public abstract partial class AIStrategy : Resource
{
    public abstract Vector2 Pathfind(Node2D agent, AIComponent aiComponent);

    public abstract Vector2 Pathfind(Vector2 target, Node2D agent, AIComponent aIComponent);

    public virtual void OnEnemyNear(Node2D agent, Node2D enemy) { }
    public virtual void OnFriendlyNear(Node2D agent, Node2D friendly) { }
    public virtual void OnEnemyLeave(Node2D agent, Node2D enemy) { }
    public virtual void OnFriendlyLeave(Node2D agent, Node2D friendly) { }

    public virtual AIStrategy OnComponentReady() => this;
    public virtual AIStrategy OnWaveSpawn() => this;
}