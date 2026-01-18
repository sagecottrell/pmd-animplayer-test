using Godot;

namespace breakout.components.scripts;

public partial class UnitClickAreaComponent : Area2D
{
    [Signal]
    public delegate void UnitClickedEventHandler(UnitClickAreaComponent unitClickAreaComponent);

    bool _startClick;
    bool _doubleClick;

    Node2D? _parent;

    public override void _EnterTree()
    {
        _parent = GetParent<Node2D>();
        InputEvent += OnClickAreaInputEvent;
    }

    public Shape2D? GetShape()
    {
        if (GetNodeOrNull<CollisionShape2D>("CollisionShape2D") is CollisionShape2D cs)
        {
            return cs.Shape;
        }
        return null;
    }

    public void OnClickAreaInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (_parent is null)
            return;
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                _startClick = true;
                _doubleClick = mouseEvent.DoubleClick;
                if (_doubleClick)
                    GlobalSignals.Instance?.DoubleClick(_parent);
            }
            else if (_startClick && !mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                _startClick = false;
                if (!_doubleClick)
                {
                    GlobalSignals.Instance?.SingleClick(_parent);
                }
                (viewport as Viewport)?.SetInputAsHandled();
            }
            EmitSignalUnitClicked(this);
        }
    }
}
