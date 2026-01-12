using Godot;

namespace breakout.components.scripts;

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
        if (!IsSelected) return;

        // draw red circle
        DrawCircle(Vector2.Zero, SelectionRadius, Colors.Red, filled: false, width: SelectionWidth);
    }
}
