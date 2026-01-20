using Godot;
using System.Collections.Generic;

namespace breakout.components.scripts;

public partial class UnitClickAreaComponent : Area2D, INodeComponent
{
    [Signal]
    public delegate void UnitClickedEventHandler(UnitClickAreaComponent unitClickAreaComponent);

    readonly HashSet<MouseButton> _startClick = [];
    readonly HashSet<MouseButton> _doubleClick = [];

    Node2D? _parent;

    public override void _EnterTree()
    {
        _parent = GetParent<Node2D>();
        InputEvent += OnClickAreaInputEvent;
    }

    public Shape2D? GetShape() => GetNodeOrNull<CollisionShape2D>("CollisionShape2D")?.Shape;

    public void OnClickAreaInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (_parent is null)
            return;
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex is MouseButton b)
        {
            if (mouseEvent.Pressed)
            {
                _startClick.Add(b);
                if (mouseEvent.DoubleClick) _doubleClick.Add(b); else _doubleClick.Remove(b);
                if (mouseEvent.DoubleClick)
                    GlobalSignals.Instance?.DoubleClick(_parent, b);
            }
            else if (_startClick.Contains(b) && !mouseEvent.Pressed)
            {
                _startClick.Remove(b);
                if (!_doubleClick.Contains(b))
                {
                    GlobalSignals.Instance?.SingleClick(_parent, b);
                }
                (viewport as Viewport)?.SetInputAsHandled();
            }
            EmitSignalUnitClicked(this);
        }
    }
}
