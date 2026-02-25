using breakout.customResources;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace breakout.components.scripts;

[GlobalClass]
public partial class UnitLevelComponent : Node, INodeComponent
{
    [Export]
    public int Level { get; set; }
    [Export]
    public Godot.Collections.Dictionary<int, MoveDefinition>? LevelupMoveset { get; set; }
    [Export]
    public MoveDefinition? Move1 { get; set; }
    [Export]
    public MoveDefinition? Move2 { get; set; }
    [Export]
    public MovePriority MovePriority { get; set; }
    [Export]
    public PokeDefinition? BaseDefinition;
    [Export]
    [ExportGroup("StatsPerLevel")]
    public float HitPoints_PL { get; set; }
    [Export]
    public float Attack_PL { get; set; }
    [Export]
    public float Defense_PL { get; set; }
    [Export]
    public float SpAttack_PL { get; set; }
    [Export]
    public float SpDefense_PL { get; set; }
    [Export]
    public float Speed_PL { get; set; }

    public int HitPoints => (int)(Level * HitPoints_PL);
    public int Attack => (int)(Level * Attack_PL);
    public int Defense => (int)(Level * Defense_PL);
    public int SpAttack => (int)(Level * SpAttack_PL);
    public int SpDefense => (int)(Level * SpDefense_PL);
    public int Speed => (int)(Level * Speed_PL);

    public MoveDefinition? GetMoveForRange(uint range)
    {
        List<MoveDefinition> moves = [Move1, Move2];
        if (MovePriority != MovePriority.First)
        {
            var e = moves.ElementAt((int)MovePriority);
            moves.RemoveAt((int)MovePriority);
            moves.Insert(0, e);
        }
        foreach (var move in moves)
        {
            if (move != null && move.Range >= range)
            {
                return move;
            }
        }
        return null;
    }
}


public enum MovePriority
{
    First = 0,
    Second = 1,
    //Third = 2,
    //Fourth = 3
}