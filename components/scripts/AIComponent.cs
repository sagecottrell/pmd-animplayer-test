using breakout.components.AIStrategies;
using breakout.components.scripts;
using Godot;
namespace breakout.components;

[GlobalClass]
public partial class AIComponent : Node, INodeComponent
{
    private AIStrategy? strategy;

    [Export]
    public AIStrategy? Strategy { get => strategy; set 
        {
            if (value is SquadStrategy ss && GetParent() is Node2D n2d)
                ss.AddUnit(n2d);
            strategy = value;
        } 
    }

    [Signal]
    public delegate void OnNewTargetPointEventHandler(Vector2 newTargetPoint);

    [Signal]
    public delegate void OnReachedTargetEventHandler(Vector2 globalPosition);

    public override void _Ready()
    {
        Strategy = Strategy?.OnComponentReady();
    }

    public void Pathfind()
    {
        if (GetParent() is not Node2D parent) 
            return;
        if (Strategy?.Pathfind(parent, this) is Vector2 v)
            EmitSignalOnNewTargetPoint(v);
    }
}

public interface IAIComponentModifier
{
}