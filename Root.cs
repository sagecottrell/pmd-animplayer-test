using breakout.components.scripts;
using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace breakout;

public partial class Root : Node2D
{
    [Export]
    public Array<Node2D> SpawnPoints = [];
    [Export]
    public PackedScene? SpawnScene;

    List<Node2D> selectedUnits = [];

    bool click_drag;
    Vector2 start_drag;
    Vector2 end_drag;

    public override void _Ready()
    {
        GetNode<Button>("%SpawnUnit").Pressed += OnSpawn;
        GetNode<Area2D>("RootClick").InputEvent += _root_InputEvent;
        GetViewport().PhysicsObjectPickingSort = true;
        GlobalSignals.Instance!.OnToggleUnitSelect += (units) =>
        {
            foreach (var unit in units)
            {
                if (!selectedUnits.Remove(unit))
                {
                    selectedUnits.Add(unit);
                    if (GetComponent.TryGetSelectableComponent(unit, out var selectable))
                        selectable.Select();
                }
                else
                    if (GetComponent.TryGetSelectableComponent(unit, out var selectable))
                        selectable.Deselect();
            }
        };
        GlobalSignals.Instance.OnUnitSelect += (units) =>
        {
            foreach (var unit in selectedUnits)
                if (GetComponent.TryGetSelectableComponent(unit, out var selectable))
                    selectable.Deselect();
            selectedUnits.Clear();
            foreach (var unit in units)
            {
                if (GetComponent.TryGetSelectableComponent(unit, out var selectable))
                {
                    selectable.Select();
                    selectedUnits.Add(unit);
                }
            }
        };
    }

    private void _root_InputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        switch (@event)
        {
            case InputEventMouseButton mouseEvent when mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left:
                end_drag = mouseEvent.GlobalPosition;
                start_drag = mouseEvent.GlobalPosition;
                break;
            case InputEventMouseButton mouseEvent when !mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left:
                click_drag = false;
                QueueRedraw();
                OnFinishSelection();
                break;
            case InputEventMouseMotion mouseMotion when Input.IsMouseButtonPressed(MouseButton.Left):
                if (!click_drag && mouseMotion.GlobalPosition.DistanceTo(start_drag) > 5.0f)
                {
                    click_drag = true;
                }
                if (click_drag)
                {
                    end_drag = mouseMotion.GlobalPosition;
                    QueueRedraw();
                }
                break;
        }
    }

    public void OnSpawn()
    {
        if (SpawnScene == null)
            return;
        var spawnPoint = SpawnPoints.PickRandom();
        var newNode = SpawnScene.Instantiate<Node2D>();
        newNode.GlobalPosition = spawnPoint.GlobalPosition;
        GetNode("%Units").AddChild(newNode);
    }

    public void OnFinishSelection()
    {
        Rect2 selectionRect = new(
            new Vector2(Mathf.Min(start_drag.X, end_drag.X), Mathf.Min(start_drag.Y, end_drag.Y)),
            new Vector2(Mathf.Abs(end_drag.X - start_drag.X), Mathf.Abs(end_drag.Y - start_drag.Y))
        );
        var units = new List<Node2D>();
        foreach (var unit in GetNode("%Units").GetChildren().Cast<Node2D>())
        {
            if (GetComponent.TryGetSelectableComponent(unit, out var selectable))
            {
                if (selectionRect.HasPoint(selectable.GlobalPosition))
                {
                    units.Add(unit);
                }
            }
        }
        GlobalSignals.Instance!.UnitSelect([..units]);
    }

    public override void _Draw()
    {
        if (click_drag)
        {
            DrawRect(new Rect2(
                new Vector2(Mathf.Min(start_drag.X, end_drag.X), Mathf.Min(start_drag.Y, end_drag.Y)),
                new Vector2(Mathf.Abs(end_drag.X - start_drag.X), Mathf.Abs(end_drag.Y - start_drag.Y))
            ), Colors.Blue, false, 2.0f);
        }
    }
}
