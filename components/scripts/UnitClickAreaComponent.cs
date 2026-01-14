using breakout.components.AIStrategies;
using Godot;

namespace breakout.components.scripts;

public partial class UnitClickAreaComponent : Area2D
{
    [Signal]
    public delegate void UnitClickedEventHandler(UnitClickAreaComponent unitClickAreaComponent);

    [Export]
    public AIComponent? AI;

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
                if (mouseEvent.DoubleClick && AI?.Strategy is SquadStrategy ss)
                {
                    GlobalSignals.Instance?.SquadSelect(ss.SquadInfo);
                }
            }
            else if (_startClick && !mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                _startClick = false;
                if (!_doubleClick)
                {
                    if (Input.IsKeyPressed(Key.Shift))
                    {
                        GlobalSignals.Instance?.ToggleUnitSelect([_parent]);
                    }
                    else
                    {
                        GlobalSignals.Instance?.UnitSelect([_parent]);
                    }
                }
                (viewport as Viewport)?.SetInputAsHandled();
            }
            EmitSignalUnitClicked(this);
        }
    }
}
