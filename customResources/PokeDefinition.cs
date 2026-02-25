using breakout.components.scripts;
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
    public LevelXPRate XPRate { get; set; } = LevelXPRate.MediumFast;

    [Export]
    [ExportGroup("StatsPerLevel")]
    public float HitPoints_PL { get; set; } = 1;
    [Export]
    public float Attack_PL { get; set; } = 1;
    [Export]
    public float Defense_PL { get; set; } = 1;
    [Export]
    public float SpAttack_PL { get; set; } = 1;
    [Export]
    public float SpDefense_PL { get; set; } = 1;
    [Export]
    public float Speed_PL { get; set; } = 1;

    static Dictionary<string, PokeDefinition>? _allDefinitions;
    public static Dictionary<string, PokeDefinition> AllDefinitions => LoadDefs.LoadAll(ref _allDefinitions, "res://poke_defs/", r => r.Name);

    public void ConfigureUnit(Node2D unit, int level)
    {
        unit.Configure<PMDSprite>(p => p.Sprites = AnimationLibrary);
        unit.Configure<PokeTypeComponent>(t =>
        {
            t.Type1 = Type1;
            t.Type2 = Type2;
            t.BaseDefinition = this;
        });
        unit.Configure<UnitLevelComponent>(m =>
        {
            m.Level = level;
            m.BaseDefinition = this;
            m.LevelupMoveset = LevelupMoveset;
            m.HitPoints_PL = HitPoints_PL;
            m.Attack_PL = Attack_PL;
            m.Defense_PL = Defense_PL;
            m.SpAttack_PL = SpAttack_PL;
            m.SpDefense_PL = SpDefense_PL;
            m.Speed_PL = Speed_PL;
        });
    }
}

public enum LevelXPRate
{
    Fast,
    MediumFast,
    MediumSlow,
    Slow,
    Erratic,
}
