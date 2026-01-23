using breakout.buildings;
using breakout.customResources;
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
    public delegate void OnDoubleClickEventHandler(Node2D unit, MouseButton button);
    [Signal]
    public delegate void OnSingleClickEventHandler(Node2D units, MouseButton button);
    [Signal]
    public delegate void OnUnitSpawnEventHandler(Godot.Collections.Array<Node2D> units);
    [Signal]
    public delegate void OnPlayerResourcesChangeEventHandler(Godot.Collections.Dictionary<GameResourceNames, long> resources);
    [Signal]
    public delegate void OnRequestBuildingCreateEventHandler(BuildableDefinition buildingDefinition);
    [Signal]
    public delegate void OnBuildingCreateEventHandler(BaseBuilding buildingDefinition);
    [Signal]
    public delegate void OnBuildingCancelEventHandler();

    public void GameOver()
    {
        EmitSignal(nameof(OnGameOver));
    }

    public void DoubleClick(Node2D unit, MouseButton button)
    {
        EmitSignalOnDoubleClick(unit, button);
    }

    public void SingleClick(Node2D units, MouseButton button)
    {
        EmitSignalOnSingleClick(units, button);
    }

    public void UnitSpawn(Godot.Collections.Array<Node2D> units)
    {
        EmitSignalOnUnitSpawn(units);
    }

    public void PlayerResourcesChange(Godot.Collections.Dictionary<GameResourceNames, long> resources)
    {
        EmitSignalOnPlayerResourcesChange(resources);
    }

    public void RequestBuildingCreate(BuildableDefinition buildingDefinition)
    {
        EmitSignalOnRequestBuildingCreate(buildingDefinition);
    }

    public void BuildingCreate(BaseBuilding buildingDefinition)
    {
        EmitSignalOnBuildingCreate(buildingDefinition);
    }

    public void BuildingCancel()
    {
        EmitSignalOnBuildingCancel();
    }
}
