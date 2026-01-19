using Godot;

namespace breakout.components.scripts;

[GlobalClass]
public partial class SelectableComponent : Node2D
{
    [Signal]
    public delegate void OnSelectionChangedEventHandler(bool isSelected);

    [Export]
    public bool CanMultiSelect = false;
    [Export]
    public bool Reselectable = true;

    [Export(PropertyHint.Range, "0,1,0.1")]
    public float UnselectedAlpha = 0f;

    [Export]
    public CollisionShape2D SelectionShape = new()
    {
        Shape = new CircleShape2D()
        {
            Radius = 16,
        },
    };

    [Export]
    public int SelectionBorderSize = 3;

    bool selected = false;
    [Export]
    public bool IsSelected
    {
        get => selected; set
        {
            if (selected != value) {
                QueueRedraw();
                selected = value;
                EmitSignalOnSelectionChanged(selected);
            }
        }
    }

    public void ToggleSelection()
    {
        IsSelected = !IsSelected;
    }

    public void Select()
    {
        IsSelected = true;
    }
    public void Deselect()
    {
        IsSelected = false;
    }

    public override void _Draw()
    {
        if (!GetComponent.TryGetTeamComponent(GetParent(), out var Team))
            return;

        // draw red circle
        var color = Team.Team?.Color ?? Colors.WebGray;
        if (!IsSelected) color.A = UnselectedAlpha;

        switch (SelectionShape.Shape) 
        { 
            case CircleShape2D circleShape:
                DrawCircle(Vector2.Zero, circleShape.Radius, color, filled: false, width: SelectionBorderSize);
                return;
            case RectangleShape2D rectShape:
                DrawRect(rectShape.GetRect(), color, false, SelectionBorderSize);
                return;
        }
    }
}
