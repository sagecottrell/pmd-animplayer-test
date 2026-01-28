using Godot;
using Godot.Collections;

namespace breakout.customResources;

[Tool]
[GlobalClass]
public partial class PokeDefinition : Resource
{
    [Export]
    public string Name { get; set; } = "";
    [Export]
    public Texture2D? Sprite { get; set; }
    [Export]
    public AnimationLibrary? AnimationLibrary { get; set; }
    [Export]
    public PokeType Type1 { get; set; } = PokeType.Normal;
    [Export]
    public PokeType Type2 { get; set; } = PokeType.None;
    [Export]
    public Dictionary<int, MoveDefinition>? LevelupMoveset { get; set; }
    [Export]
    public Array<PokeDefinition> Evolutions { get; set; } = [];

    [Export]
    public int Stat_HP { get; set; } = 10;
    [Export]
    public int Stat_Attack { get; set; } = 10;
    [Export]
    public int Stat_Defense { get; set; } = 10;
    [Export]
    public int Stat_SpAttack { get; set; } = 10;
    [Export]
    public int Stat_SpDefense { get; set; } = 10;
    [Export]
    public int Stat_Speed { get; set; } = 10;
}
