using breakout.components.AIStrategies;
using Godot;
namespace breakout.components;

[GlobalClass]
public partial class AIComponent : BaseComponent
{
    [Export]
    public Node2D? Target { get; set; }

    [Export]
    public AIStrategy? Strategy { get; set; }

    [Signal]
    public delegate void OnNewVelocityEventHandler(Vector2 velocity);

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
        if (GetParent() is not Node2D parent) return;
        if (Strategy?.Pathfind(parent, this) is Vector2 v)
            EmitSignalOnNewVelocity(v);
    }
}

public interface IAIComponentModifier
{
}