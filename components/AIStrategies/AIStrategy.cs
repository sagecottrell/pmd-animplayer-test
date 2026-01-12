using Godot;

namespace breakout.components.AIStrategies;

[GlobalClass]
public abstract partial class AIStrategy : Resource
{
    public virtual bool CreateCopyOnComponentReady => false;
    public abstract Vector2 Pathfind(Node2D agent, AIComponent aiComponent);
}
