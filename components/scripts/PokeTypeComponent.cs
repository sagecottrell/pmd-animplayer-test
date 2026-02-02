using breakout.customResources;
using Godot;

namespace breakout.components.scripts;

[Tool]
[GlobalClass]
public partial class PokeTypeComponent : Node, INodeComponent
{
    [Export]
    public PokeType Type1 { get; set; } = PokeType.Normal;
    [Export]
    public PokeType Type2 { get; set; } = PokeType.None;
}

public interface IPokeTypeComponentModifier
{
}