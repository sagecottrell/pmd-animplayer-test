using Godot;

namespace breakout.components.AIStrategies;

[Tool]
[GlobalClass]
public abstract partial class AIStrategy : Resource
{
    public virtual bool CreateCopyOnComponentReady => false;
    public Vector2 Pathfind(Node2D agent, AIComponent aiComponent) => StrategyType switch
    {
        AIStrategyType.FollowTarget => Follow(agent, aiComponent),
        AIStrategyType.AttackTarget => Attack(agent, aiComponent),
        AIStrategyType.FleeTarget => Flee(agent, aiComponent),
        _ => throw new System.Exception($"{nameof(AIStrategy)} - Invalid StrategyType: {StrategyType}"),
    };

    protected virtual Vector2 Follow(Node2D agent, AIComponent aiComponent) => Vector2.Zero;
    protected virtual Vector2 Attack(Node2D agent, AIComponent aiComponent) => Vector2.Zero;
    protected virtual Vector2 Flee(Node2D agent, AIComponent aiComponent) => Vector2.Zero;

    [Export]
    public AIStrategyType StrategyType { get; set; } = AIStrategyType.FollowTarget;
}

public enum AIStrategyType
{
    FollowTarget = 1,
    AttackTarget = 2,
    FleeTarget = 3,
}