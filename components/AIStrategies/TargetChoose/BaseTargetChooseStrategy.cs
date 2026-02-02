using Godot;

namespace breakout.components.AIStrategies.TargetChoose;

[Tool]
[GlobalClass]
public abstract partial class BaseTargetChooseStrategy : Resource
{
    public abstract Node2D? GetTarget(Node2D unit);
}
