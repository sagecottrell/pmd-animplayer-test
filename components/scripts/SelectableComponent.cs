using breakout.resourceTypes;
using Godot;

namespace breakout.components.scripts;

[GlobalClass]
public partial class SelectableComponent : Node2D
{
    [Signal]
    public delegate void OnSelectionChangedEventHandler(bool isSelected);

    [Export]
    public float SelectionRadius = 40.0f;

    [Export]
    public float SelectionWidth = 10.0f;

    bool selected = false;
    [Export]
    public bool IsSelected
    {
        get => selected; set
        {
            if (selected != value) QueueRedraw();
            selected = value;
            EmitSignalOnSelectionChanged(selected);
        }
    }

    public void ToggleSelection()
    {
        IsSelected = !IsSelected;
    }

    public void Select()
    {
        IsSelected = true;
        // You can add visual feedback for selection here
    }
    public void Deselect()
    {
        IsSelected = false;
        // You can remove visual feedback for selection here
    }

    public override void _Draw()
    {

        // draw red circle
        var color = Colors.Red;
        if (GetComponent.TryGetTeamComponent(GetParent(), out var team))
        {
            color = team.Team?.Color ?? color;
        }
        if (!IsSelected) color.A = 0.3f;

        DrawCircle(Vector2.Zero, SelectionRadius, color, filled: false, width: SelectionWidth);
    }
}
