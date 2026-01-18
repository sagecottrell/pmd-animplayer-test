using Godot;

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
    public delegate void OnDoubleClickEventHandler(Node2D unit);
    [Signal]
    public delegate void OnSingleClickEventHandler(Node2D units);

    public void GameOver()
    {
        EmitSignal(nameof(OnGameOver));
    }

    public void DoubleClick(Node2D unit)
    {
        EmitSignalOnDoubleClick(unit);
    }

    public void SingleClick(Node2D units)
    {
        EmitSignalOnSingleClick(units);
    }
}
