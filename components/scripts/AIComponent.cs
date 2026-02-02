using breakout.components.AIStrategies;
using breakout.components.AIStrategies.TargetChoose;
using breakout.components.scripts;
using Godot;
namespace breakout.components;

[GlobalClass]
public partial class AIComponent : Node, INodeComponent
{
    [Export]
    public Node2D? Target { get; set; }

    [Export]
    public AIStrategy? Strategy { get; set; }

    public bool HasReachedTarget = false;

    [Export]
    public BaseTargetChooseStrategy? TargetChooseStrategy { get; set; }

    [Signal]
    public delegate void OnNewVelocityEventHandler(Vector2 velocity);

    [Signal]
    public delegate void OnReachedTargetEventHandler();

    public override void _Ready()
    {
        Strategy = Strategy?.CreateCopyOnComponentReady != true ? Strategy : Strategy?.Duplicate() as AIStrategy;
    }

    public override void _Process(double delta)
    {
        Pathfind();
    }

    public void Pathfind()
    {
        if (GetParent() is not Node2D parent) 
            return;
        if (Target is null && TargetChooseStrategy is not null) 
            Target = TargetChooseStrategy.GetTarget(parent);
        if (Strategy?.Pathfind(parent, this) is Vector2 v)
            EmitSignalOnNewVelocity(v);
    }

    public void ReachedTarget()
    {
        if (!HasReachedTarget)
        {
            HasReachedTarget = true;
            EmitSignalOnReachedTarget();
        }
    }
}

public interface IAIComponentModifier
{
}