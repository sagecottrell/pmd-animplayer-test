using breakout.components.scripts;
using Godot;
using Godot.Collections;

namespace breakout;

public partial class GlobalSignals : Node
{

    private static GlobalSignals? _instance;
    public static GlobalSignals? Instance => _instance;

    // Use _EnterTree to make sure the Singleton instance is avaiable in _Ready()
    public override void _EnterTree()
    {
        if (_instance != null)
        {
            QueueFree(); // The Singleton is already loaded, kill this instance
        }
        _instance = this;
    }

    [Signal]
    public delegate void OnGameOverEventHandler();
    [Signal]
    public delegate void OnSquadSelectEventHandler(SquadInfo squad);
    [Signal]
    public delegate void OnUnitSelectEventHandler(Array<Node2D> units);
    [Signal]
    public delegate void OnToggleUnitSelectEventHandler(Array<Node2D> units);
    [Signal]
    public delegate void OnPrimaryActionEventHandler();

    public void GameOver()
    {
        EmitSignal(nameof(OnGameOver));
    }

    public void SquadSelect(SquadInfo squad)
    {
        EmitSignalOnSquadSelect(squad);
    }

    public void PrimaryAction()
    {
        EmitSignalOnPrimaryAction();
    }

    public void UnitSelect(Array<Node2D> units)
    {
        EmitSignalOnUnitSelect(units);
    }

    public void ToggleUnitSelect(Array<Node2D> units)
    {
        EmitSignalOnToggleUnitSelect(units);
    }
}
