using Godot;

namespace breakout.customResources;

[Tool]
[GlobalClass]
public partial class MoveDefinition : Resource
{
    [Export]
    public string Name { get; set; } = "";
    [Export]
    public PackedScene? AnimationScene { get; set; }
    [Export]
    public int Power { get; set; } = 0;
    [Export]
    public int Accuracy { get; set; } = 100;
    [Export]
    public int MaxPP { get; set; } = 10;
    [Export]
    public bool IsSpecial { get; set; } = false;
    [Export]
    public PokeType Type { get; set; } = PokeType.Normal;
}
