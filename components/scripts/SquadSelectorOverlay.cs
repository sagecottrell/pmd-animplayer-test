
using breakout.components.AIStrategies;
using breakout.customResources;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace breakout.components.scripts;

[GlobalClass]
public partial class SquadSelectorOverlay : Area2D
{
    TeamInfo? PlayerTeam;
    List<Node2D> selectedUnits = [];
    SquadStrategy? selectedSquad;

    [Signal]
    public delegate void OnSquadSetPositionEventHandler(SquadStrategy squadInfo, Vector2 squadGlobalPosition, TeamInfo team);

    [Signal]
    public delegate void OnSquadSelectedEventHandler(SquadStrategy? squadInfo);

    public bool Enabled;

    bool m1_click_drag;
    Vector2? m1_start_drag;
    Vector2? m1_end_drag;

    bool m2_click_drag;
    Vector2? m2_start_drag;
    Vector2? m2_end_drag;

    public override void _Ready()
    {
        if (this.TryGetContext<IGameState>(out var gameState))
        {
            PlayerTeam = gameState.PlayerTeam;
        }

        InputEvent += _on_InputEvent;
        if (GlobalSignals.Instance is not null)
        {
            GlobalSignals.Instance.OnSingleClick += _on_SingleClick;
            GlobalSignals.Instance.OnDoubleClick += _on_DoubleClick;
        }
    }

    private void _on_InputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (!Enabled) return;
        switch (@event)
        {
            case InputEventMouseButton mouseEvent when mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left:
                m1_end_drag = m1_start_drag = GetGlobalMousePosition();
                break;
            case InputEventMouseButton mouseEvent when !mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left:
                m1_click_drag = false;
                QueueRedraw();
                OnFinishSelection();
                m1_end_drag = m1_start_drag = null;
                break;
            case InputEventMouseMotion when Input.IsMouseButtonPressed(MouseButton.Left) && m1_start_drag.HasValue:
                if (!m1_click_drag && GetGlobalMousePosition().DistanceTo(m1_start_drag.Value) > 5.0f)
                {
                    m1_click_drag = true;
                }
                if (m1_click_drag)
                {
                    m1_end_drag = GetGlobalMousePosition();
                    QueueRedraw();
                }
                break;
            case InputEventMouseButton when selectedUnits.Count == 0:
                break;
            case InputEventMouseButton mouseEvent when mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Right:
                m2_end_drag = m2_start_drag = GetGlobalMousePosition();
                m2_click_drag = true;
                break;
            case InputEventMouseButton mouseEvent when !mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Right && m2_start_drag.HasValue:
                m2_click_drag = false;
                QueueRedraw();
                NewOrExistingSquad(m2_start_drag.Value);
                m2_start_drag = m2_end_drag = null;
                break;
            case InputEventMouseMotion when Input.IsMouseButtonPressed(MouseButton.Right):
                m2_end_drag = GetGlobalMousePosition();
                QueueRedraw();
                break;
        }
    }

    private void _on_SingleClick(Node2D unit, MouseButton button)
    {
        if (!Enabled) return;
        if (button == MouseButton.Left)
        {
            if (Input.IsKeyPressed(Key.Shift))
                // add clicked units to selection
                _toggleUnit([unit]);
            else
                _selectUnit([unit]);
            CheckAllSameSquad();
            EmitSignalOnSquadSelected(selectedSquad);
        }
    }

    private void _on_DoubleClick(Node2D unit, MouseButton b)
    {
        if (!Enabled) return;
        if (b == MouseButton.Left && GetComponent.TryGetAIComponent(unit, out var ai) && ai?.Strategy is SquadStrategy ss)
        {
            _selectUnit(this.SquadMembers(ss));
        }
    }

    private void _toggleUnit(Godot.Collections.Array<Node2D> units)
    {
        Godot.Collections.Array<Node2D> selection = [..selectedUnits]; // duplicating this list so we can toggle
        foreach (var unit in units)
        {
            if (!selection.Remove(unit))
                selection.Add(unit);
        }
        _selectUnit(selection);
    }

    private void _selectUnit(Godot.Collections.Array<Node2D> units)
    {
        selectedSquad = null;
        foreach (var unit in selectedUnits)
            if (unit.TryGetComponent<SelectableComponent>(out var selectable) && selectable.IsSelected && (!units.Contains(unit) || selectable.Reselectable))
                selectable.Deselect();
        selectedUnits.Clear();
        foreach (var unit in units)
        {
            // select unit only if it can be multi-selected or if it is the only unit selected
            if (unit.TryGetComponent<SelectableComponent>(out var selectable) && selectable.IsSelected == false && (selectable.CanMultiSelect || units.Count == 1))
            {
                var teamMatch = !unit.TryGetComponent<TeamComponent>(out var team) || team.Team is null || team.Team == PlayerTeam;
                if (!teamMatch) continue;
                selectable.Select();
                selectedUnits.Add(unit);
            }
        }
        CheckAllSameSquad();
        EmitSignalOnSquadSelected(selectedSquad);
    }

    public void OnFinishSelection()
    {
        if (m1_start_drag is not Vector2 start || m1_end_drag is not Vector2 end)
            return;

        Rect2 selectionRect = new(
            new Vector2(Mathf.Min(start.X, end.X), Mathf.Min(start.Y, end.Y)),
            new Vector2(Mathf.Abs(end.X - start.X), Mathf.Abs(end.Y - start.Y))
        );
        var units = new List<Node2D>();
        foreach (var unit in GetNode("%Units").GetChildren().Cast<Node2D>())
        {
            if (unit.TryGetComponent<SelectableComponent>(out var selectable))
            {
                if (selectionRect.HasPoint(selectable.GlobalPosition))
                {
                    units.Add(unit);
                }
            }
        }
        _selectUnit([.. units]);
    }

    public void NewOrExistingSquad(Vector2 position)
    {
        if (selectedSquad is null)
            CheckAllSameSquad();

        if (m2_start_drag is not Vector2 start || m2_end_drag is not Vector2 end)
            return;

        var facingDirection = end.IsEqualApprox(start) ? Vector2.Up : (end - start).Normalized();
        // if the selected squad matches the selected units, return it
        if (selectedSquad is not null && selectedSquad.PlayerControllable && this.SquadMembers(selectedSquad).ToHashSet().SetEquals(selectedUnits))
        {
#if TOOLS
            GD.Print("Reusing existing squad");
#endif
            selectedSquad.FacingDirection = facingDirection;
            EmitSignalOnSquadSetPosition(selectedSquad, position, PlayerTeam);
            return;
        }

        // set up a new squad
        GD.Print($"Setup new squad for {selectedUnits.Count} units");
        var strategy = new SquadStrategy
        {
            FacingDirection = facingDirection,
            PlayerControllable = true,
        };

        if (selectedSquad is not null)
        {
            strategy.StrategyApproach = selectedSquad.StrategyApproach;
            strategy.StrategyAttack = selectedSquad.StrategyAttack;
            strategy.TargetChooseStrategy = selectedSquad.TargetChooseStrategy;
        }

        foreach (var unit in selectedUnits)
        {
            if (GetComponent.TryGetAIComponent(unit, out var ai))
                ai.Strategy = strategy;
        }
        selectedSquad = strategy;
        GlobalSignals.Instance?.SquadCreate(strategy);
        EmitSignalOnSquadSetPosition(selectedSquad, position, PlayerTeam);
    }

    public override void _Draw()
    {
        DrawSetTransform(-GlobalPosition);
        if (m1_click_drag)
        {
            if (m1_start_drag is not Vector2 start || m1_end_drag is not Vector2 end)
                return;
            DrawRect(new Rect2(
                new Vector2(Mathf.Min(start.X, end.X), Mathf.Min(start.Y, end.Y)),
                new Vector2(Mathf.Abs(end.X - start.X), Mathf.Abs(end.Y - start.Y))
            ), PlayerTeam?.Color ?? Colors.Red, false, 2.0f);
        }
        if (m2_click_drag)
        {
            if (m2_start_drag is not Vector2 start || m2_end_drag is not Vector2 end)
                return;
            var dist = end.DistanceTo(start);
            var dir = (end - start).Normalized();
            var end_pos = start + dir * Mathf.Min(dist, 100f);
            DrawDashedLine(start, end_pos, PlayerTeam?.Color ?? Colors.Blue, 2.0f);
            DrawCircle(start, 10.0f, PlayerTeam?.Color ?? Colors.Blue, filled: false, width: 2.0f);
            // draw arrowhead
            var perp = new Vector2(-dir.Y, dir.X);
            DrawLine(end, end_pos - dir * 20 + perp * 10, PlayerTeam?.Color ?? Colors.Blue, 2.0f);
            DrawLine(end, end_pos - dir * 20 - perp * 10, PlayerTeam?.Color ?? Colors.Blue, 2.0f);
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
            selectedSquad = ss;
        }
    }
}
