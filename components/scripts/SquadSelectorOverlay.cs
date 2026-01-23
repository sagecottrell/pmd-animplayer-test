
using breakout.components.AIStrategies;
using breakout.customResources;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace breakout.components.scripts;

[GlobalClass]
public partial class SquadSelectorOverlay : Area2D
{
    TeamInfo? PlayerTeam;
    List<Node2D> selectedUnits = [];
    SquadInfo? selectedSquad;

    [Signal]
    public delegate void OnSquadSetPositionEventHandler(SquadInfo squadInfo, Vector2 squadGlobalPosition, TeamInfo team);

    [Signal]
    public delegate void OnSquadSelectedEventHandler(SquadInfo? squadInfo);

    public bool Enabled;

    bool m1_click_drag;
    Vector2 m1_start_drag;
    Vector2 m1_end_drag;

    bool m2_click_drag;
    Vector2 m2_start_drag;
    Vector2 m2_end_drag;

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
                m1_end_drag = mouseEvent.GlobalPosition;
                m1_start_drag = mouseEvent.GlobalPosition;
                break;
            case InputEventMouseButton mouseEvent when !mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left:
                m1_click_drag = false;
                QueueRedraw();
                OnFinishSelection();
                break;
            case InputEventMouseMotion mouseMotion when Input.IsMouseButtonPressed(MouseButton.Left):
                if (!m1_click_drag && mouseMotion.GlobalPosition.DistanceTo(m1_start_drag) > 5.0f)
                {
                    m1_click_drag = true;
                }
                if (m1_click_drag)
                {
                    m1_end_drag = mouseMotion.GlobalPosition;
                    QueueRedraw();
                }
                break;
            case InputEventMouseButton when selectedUnits.Count == 0:
                break;
            case InputEventMouseButton mouseEvent when mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Right:
                m2_end_drag = mouseEvent.GlobalPosition;
                m2_start_drag = mouseEvent.GlobalPosition;
                m2_click_drag = true;
                break;
            case InputEventMouseButton mouseEvent when !mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Right:
                m2_click_drag = false;
                QueueRedraw();
                NewOrExistingSquad(m2_start_drag);
                break;
            case InputEventMouseMotion mouseMotion when Input.IsMouseButtonPressed(MouseButton.Right):
                m2_end_drag = mouseMotion.GlobalPosition;
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
        if (b == MouseButton.Left && GetComponent.TryGetAIComponent(unit, out var ai) && ai?.Strategy is SquadStrategy ss && ss.SquadInfo is SquadInfo squad)
        {
            _selectUnit([.. squad.Members.Keys.Select(GetTree().Root.GetNode<Node2D>)]);
        }
    }

    private void _toggleUnit(List<Node2D> units)
    {
        List<Node2D> selection = [..selectedUnits];
        foreach (var unit in units)
        {
            if (!selection.Remove(unit))
                selection.Add(unit);
        }
        _selectUnit(selection);
    }

    private void _selectUnit(List<Node2D> units)
    {
        selectedSquad = null;
        foreach (var unit in selectedUnits)
            if (GetComponent.TryGetSelectableComponent(unit, out var selectable) && selectable.IsSelected && (!units.Contains(unit) || selectable.Reselectable))
                selectable.Deselect();
        selectedUnits.Clear();
        foreach (var unit in units)
        {
            // select unit only if it can be multi-selected or if it is the only unit selected
            if (GetComponent.TryGetSelectableComponent(unit, out var selectable) && selectable.IsSelected == false && (selectable.CanMultiSelect || units.Count == 1))
            {
                var teamMatch = !GetComponent.TryGetTeamComponent(unit, out var team) || team.Team is null || team.Team == PlayerTeam;
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
        Rect2 selectionRect = new(
            new Vector2(Mathf.Min(m1_start_drag.X, m1_end_drag.X), Mathf.Min(m1_start_drag.Y, m1_end_drag.Y)),
            new Vector2(Mathf.Abs(m1_end_drag.X - m1_start_drag.X), Mathf.Abs(m1_end_drag.Y - m1_start_drag.Y))
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
        _selectUnit([.. units]);
    }

    public void NewOrExistingSquad(Vector2 position)
    {
        if (selectedSquad is null)
            CheckAllSameSquad();

        var facingDirection = m2_end_drag.IsEqualApprox(m2_start_drag) ? Vector2.Up : (m2_end_drag - m2_start_drag).Normalized();
        // if the selected squad matches the selected units, return it
        if (selectedSquad is not null && selectedSquad.Members.Keys.ToHashSet().SetEquals(selectedUnits.Select(x => x.GetPath()).ToHashSet()))
        {
#if TOOLS
            GD.Print($"Reusing existing squad with {selectedSquad.Members.Count} members");
#endif
            selectedSquad.FacingDirection = facingDirection;
            EmitSignalOnSquadSetPosition(selectedSquad, position, PlayerTeam);
            return;
        }

        // set up a new squad
        var squad_info = new SquadInfo
        {
            FacingDirection = facingDirection,
        };
        var strategy = new SquadStrategy
        {
            SquadInfo = squad_info,
        };

        foreach (var unit in selectedUnits)
        {
            if (GetComponent.TryGetAIComponent(unit, out var ai) && ai.Strategy is SquadStrategy ss && ss.SquadInfo is not null)
            {
                squad_info.AddUnit(unit, ss.SquadInfo.UnitRanks.TryGetValue(unit.GetPath(), out var rank) ? rank : SquadRank.Frontline);
                ss.SquadInfo.RemoveUnit(unit);
                ai.Strategy = strategy;
            }
            else
                squad_info.AddUnit(unit, SquadRank.Frontline);
        }
        selectedSquad = squad_info;
        EmitSignalOnSquadSetPosition(selectedSquad, position, PlayerTeam);
    }

    public override void _Draw()
    {
        if (m1_click_drag)
        {
            DrawRect(new Rect2(
                new Vector2(Mathf.Min(m1_start_drag.X, m1_end_drag.X), Mathf.Min(m1_start_drag.Y, m1_end_drag.Y)),
                new Vector2(Mathf.Abs(m1_end_drag.X - m1_start_drag.X), Mathf.Abs(m1_end_drag.Y - m1_start_drag.Y))
            ), PlayerTeam?.Color ?? Colors.Red, false, 2.0f);
        }
        if (m2_click_drag)
        {
            var dist = m2_end_drag.DistanceTo(m2_start_drag);
            var dir = (m2_end_drag - m2_start_drag).Normalized();
            var end = m2_start_drag + dir * Mathf.Min(dist, 100f);
            DrawDashedLine(m2_start_drag, end, PlayerTeam?.Color ?? Colors.Blue, 2.0f);
            DrawCircle(m2_start_drag, 10.0f, PlayerTeam?.Color ?? Colors.Blue, filled: false, width: 2.0f);
            // draw arrowhead
            var perp = new Vector2(-dir.Y, dir.X);
            DrawLine(end, end - dir * 20 + perp * 10, PlayerTeam?.Color ?? Colors.Blue, 2.0f);
            DrawLine(end, end - dir * 20 - perp * 10, PlayerTeam?.Color ?? Colors.Blue, 2.0f);
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
