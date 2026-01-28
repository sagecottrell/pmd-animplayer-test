using breakout.components;
using Godot;
using Godot.Collections;

namespace breakout.customResources;

[Tool]
[GlobalClass]
public partial class WaveMember : Resource
{
    [Export]
    public PokeDefinition? PokeDefinition { get; set; }
    [Export]
    public int Level { get; set; } = 5;

    /// <summary>
    /// Reward for defeating this unit.
    /// </summary>
    [Export]
    public Dictionary<GameResourceNames, int> Reward { get; set; } = [];

    [Export]
    public int Quantity { get; set; } = 1;
    [Export]
    public float SpawnDelay { get; set; } = 0f;
    [Export]
    public float SpawnInterval { get; set; } = 0.5f;

    /// <summary>
    /// Modifiers to apply to this unit when spawned.
    /// </summary>
    [Export]
    public Array<BaseModifier> Modifiers { get; set; } = [];

    [Export]
    public bool IsShiny { get; set; } = false;

    /// <summary>
    /// A set of moves that will override the normal moveset for this Pokémon. use <see cref="ForcedMoveset_Count"/> to determine how many moves to randomly use from this list.
    /// </summary>
    [Export]
    public Array<MoveDefinition> ForcedMoveset { get; set; } = [];

    /// <summary>
    /// Determines how many moves to randomly select from the <see cref="ForcedMoveset"/>. If 0, the normal moveset is used.
    /// </summary>
    [Export(PropertyHint.Range, "0,4,1")]
    public int ForcedMoveset_Count { get; set; } = 0;
}