using breakout.customResources;
using Godot;
using Godot.Collections;
using System;
using System.Linq;

namespace breakout.enemySpawn.spawnStrategies;


public enum Quantifiers
{
    All,
    Partial,
    None,
}

public enum BlockMode
{
    /// <summary>
    /// Nodes with Visible = false are blocked.
    /// </summary>
    Invisible,
}

[Tool]
[GlobalClass]
public partial class SpawnBlockedRemoteStrategy : BaseSpawnStrategy
{
    [Signal]
    public delegate void OnChangedEventHandler();

    [Export]
    public Color SpawnBlockColor;
    [Export]
    public Color SpawnAllowColor;

    public override void DrawDebug(Node2D node)
    {
        if (GetParent() is not Node2D parent)
            return;
        var arrowWidth = 20;
        var arrowHeight = 40;
        var o = parent.ToLocal(node.GlobalPosition);
        parent.DrawCircle(o, 6, Colors.Green);
        foreach (var blocker in SpawnBlocker)
        {
            var pos = parent.ToLocal(blocker.GlobalPosition);
            parent.DrawCircle(pos, 8, SpawnBlockColor, filled: false);
            if (node == blocker) continue;
            parent.DrawArrow(pos, (pos - o).Normalized(), SpawnBlockColor, arrowHeight, arrowWidth, origin: o, line_dashed: SpawnBlocker_Mode == Quantifiers.None ? 4 : 0, line_width: SpawnBlocker_Mode == Quantifiers.All ? 10 : 2);
        }
        foreach (var allow in SpawnAllow)
        {
            var pos = parent.ToLocal(allow.GlobalPosition);
            parent.DrawCircle(pos, 8, SpawnAllowColor, filled: false);
            if (node == allow) continue;
            parent.DrawArrow(pos, (pos - o).Normalized(), SpawnAllowColor, arrowHeight, arrowWidth, origin: o, line_dashed: SpawnAllow_Mode == Quantifiers.None ? 7 : 0, line_width: SpawnAllow_Mode == Quantifiers.All ? 10 : 2);
        }
    }

    [Export]
    public BlockMode Block_Mode { get => block_mode; set { block_mode = value; EmitSignalOnChanged(); } }
    private BlockMode block_mode;

    /// <summary>
    /// Nodes whose position are checked to block spawning.
    /// </summary>
    [Export]
    public Array<Node2D> SpawnBlocker { get; set; } = [];

    [Export]
    public Quantifiers SpawnBlocker_Mode { get => sbmode; set { sbmode = value; EmitSignalOnChanged(); } }
    Quantifiers sbmode = Quantifiers.All;

    /// <summary>
    /// Nodes whose position are checked to allow spawning.
    /// </summary>
    [Export]
    public Array<Node2D> SpawnAllow { get; set; } = [];

    [Export]
    public Quantifiers SpawnAllow_Mode { get => samode; set { samode = value; EmitSignalOnChanged(); } }
    Quantifiers samode = Quantifiers.All;

    public override bool CanSpawnEnemies(DateTime _)
    {
        return _checkQuantifier(SpawnBlocker_Mode, SpawnBlocker.Select(_isBlocked).Count(), SpawnBlocker.Count)
            && _checkQuantifier(SpawnAllow_Mode, SpawnAllow.Select(x => !_isBlocked(x)).Count(), SpawnAllow.Count);
    }

    private static bool _checkQuantifier(Quantifiers mode, int count, int total)
    {
        return total == 0 || mode switch
        {
            Quantifiers.All => count == total,
            Quantifiers.Partial => count != total && count > 0,
            Quantifiers.None => count == 0,
            _ => false,
        };
    }

    private bool _isBlocked(Node2D node)
    {
        return Block_Mode switch
        {
            BlockMode.Invisible => !node.IsInsideTree() || !node.Visible,
            _ => false,
        };
    }
}
