using breakout.customResources;
using Godot;

namespace breakout.buildings;

public enum BuildingPlacementState
{
    Idle,
    Placing,
}

[GlobalClass]
public partial class BuildingPlacer : Node2D
{
    public BuildingPlacementState CurrentState { get; private set; } = BuildingPlacementState.Idle;
    Node2D? Hologram;
    BuildableDefinition? CurrentDefinition;

    public void StartPlacingBuilding(BuildableDefinition def)
    {
        if (CurrentState != BuildingPlacementState.Idle)
        {
            GD.PrintErr("Cannot start placing a building while already placing one.");
            return;
        }
        if (def.BuildingScene is not PackedScene scene)
        {
            GD.PrintErr("Building scene is null for definition: " + def.Name);
            return;
        }
        Hologram = scene.Instantiate<Node2D>();
        Hologram.Modulate = new Color(1, 1, 1, 0.5f); // Make it semi-transparent
        var h = GetNode<Node2D>("Holograms");
        h.AddChild(Hologram);
        CurrentState = BuildingPlacementState.Placing;
        CurrentDefinition = def;
    }

    public override void _Process(double delta)
    {
        if (CurrentState != BuildingPlacementState.Placing || Hologram is null || CurrentDefinition?.BuildingScene is null)
            return;
        Hologram.GlobalPosition = GetGlobalMousePosition();

        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            // Logic to finalize building placement
            Hologram.QueueFree();
            var newBuilding = CurrentDefinition.BuildingScene.Instantiate<BaseBuilding>();
            AddChild(newBuilding);
            newBuilding.GlobalPosition = Hologram.GlobalPosition;
            newBuilding.Owner = this;
            GlobalSignals.Instance?.BuildingCreate(newBuilding);
            CurrentState = BuildingPlacementState.Idle;
            CurrentDefinition = null;
            Hologram = null;
        }
    }
}
