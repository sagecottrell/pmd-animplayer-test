using breakout.components.AIStrategies;
using breakout.components.scripts;
using breakout.resourceTypes;
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

    [Export]
    public TeamInfo PlayerTeam;

    List<Node2D> selectedUnits = [];
    SquadInfo? selectedSquad;

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
            selectedSquad = null;
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
            CheckAllSameSquad();
            ShowSquadTargetPosition();
        };
        GlobalSignals.Instance.OnUnitSelect += (units) =>
        {
            selectedSquad = null;
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
            CheckAllSameSquad();
            ShowSquadTargetPosition();
        };
        GlobalSignals.Instance.OnSquadSelect += _instance_OnSquadSelect;
    }

    private void _instance_OnSquadSelect(SquadInfo squad)
    {
        GlobalSignals.Instance?.UnitSelect([.. squad.Members.Keys.Select(GetTree().Root.GetNode<Node2D>)]);
        selectedSquad = squad;
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
            case InputEventMouseButton mouseEvent when mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Right:
                NewOrExistingSquad().SetTargetPosition(mouseEvent.GlobalPosition);
                ShowSquadTargetPosition();
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
        if (GetComponent.TryGetTeamComponent(newNode, out var teamComponent))
            teamComponent.SetTeam(PlayerTeam);
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
        GlobalSignals.Instance!.UnitSelect([.. units]);
    }

    public SquadInfo NewOrExistingSquad()
    {
        if (selectedSquad is null)
            CheckAllSameSquad();

        // if the selected squad matches the selected units, return it
        if (selectedSquad is not null && selectedSquad.Members.Keys.ToHashSet().SetEquals(selectedUnits.Select(x => x.GetPath()).ToHashSet()))
        {
#if TOOLS
            GD.Print($"Reusing existing squad with {selectedSquad.Members.Count} members");
#endif
            return selectedSquad;
        }

        // set up a new squad
        var squad_info = new SquadInfo();
        var strategy = new SquadStrategy
        {
            SquadInfo = squad_info
        };

        foreach (var unit in selectedUnits)
        {
            if (GetComponent.TryGetAIComponent(unit, out var ai) && ai.Strategy is SquadStrategy ss)
            {
                ss.SquadInfo.RemoveUnit(unit);
            }
            ai.Strategy = strategy;
            squad_info.AddUnit(unit);
        }
        selectedSquad = squad_info;
        return squad_info;
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

    public void ShowSquadTargetPosition()
    {
        var flag = GetNode<SquadFlag>("%SquadFlag");
        if (selectedSquad is not null)
        {
            flag.Visible = true;
            flag.GlobalPosition = selectedSquad.TargetPosition;
            flag.SetTeam(PlayerTeam);
        }
        else
        {
            flag.Visible = false;
        }
    }

    public void CheckAllSameSquad()
    {
        // check if all the selected members are already in the same squad
        var s = selectedUnits.Select(x => GetComponent.TryGetAIComponent(x, out var ai) && ai.Strategy is SquadStrategy ss ? ss : null)
            .Distinct()
            .ToList();
        if (s.Count == 1 && s[0] is SquadStrategy ss)
        {
            selectedSquad = ss.SquadInfo;
        }
    }
}
