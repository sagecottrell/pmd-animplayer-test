using breakout.components.scripts;
using breakout.customResources;
using Godot;

namespace breakout.moves.components;

[Tool]
[GlobalClass]
public partial class CircleComponent : Node2D, INodeComponent
{
    private float radius = 50.0f;
    private Color color = Colors.Red;
    private PokeTypeComponent? typeComponent;

    [Export]
    public float Radius { get => radius; set { radius = value; QueueRedraw(); } }
    [Export]
    public Color Color { get => color; set { color = value; QueueRedraw(); } }
    [Export]
    public PokeTypeComponent? TypeComponent { get => typeComponent; set
        {
            if (typeComponent?.Type1.GetColor() is Color color)
                Color = color;
            typeComponent = value; 
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        if (typeComponent?.Type1.GetColor() is Color c)
            color = c;
        DrawCircle(Vector2.Zero, Radius, Color);
    }
}

public interface ICircleComponentModifier
{
}