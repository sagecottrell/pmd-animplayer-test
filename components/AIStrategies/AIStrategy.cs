using Godot;

namespace breakout.components.AIStrategies;

[GlobalClass]
public abstract partial class AIStrategy : Resource
{
    public virtual bool Singleton => true;
    public abstract Vector2 Pathfind(Node2D agent, Node2D target);
}
