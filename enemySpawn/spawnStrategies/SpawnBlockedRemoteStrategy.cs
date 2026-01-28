using Godot;
using Godot.Collections;
using System.Linq;

namespace breakout.enemySpawn.spawnStrategies;


public enum Quantifiers
{
    All,
    Some,
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

    public override void DrawDebug()
    {
        var node = GetParent<Node2D>();
        var arrowWidth = 20;
        var arrowHeight = 40;
        node.DrawCircle(Vector2.Zero, 6, Colors.Green);
        foreach (var blocker in SpawnBlocker)
        {
            var pos = node.ToLocal(blocker.GlobalPosition);
            node.DrawCircle(pos, 8, SpawnBlockColor, filled: false);
            if (node == blocker || SpawnBlocker_Mode == Quantifiers.None) continue;
            node.DrawArrow(pos, pos.Normalized(), SpawnBlockColor, arrowHeight, arrowWidth, origin: Vector2.Zero, line_dashed: SpawnBlocker_Mode == Quantifiers.Some ? 4 : 0);
        }
        foreach (var allow in SpawnAllow)
        {
            var pos = node.ToLocal(allow.GlobalPosition);
            node.DrawCircle(pos, 8, SpawnAllowColor, filled: false);
            if (node == allow || SpawnBlocker_Mode == Quantifiers.None) continue;
            node.DrawArrow(pos, pos.Normalized(), SpawnAllowColor, arrowHeight, arrowWidth, origin: Vector2.Zero, line_dashed: SpawnAllow_Mode == Quantifiers.Some ? 4 : 0);
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

    public override bool CanSpawnEnemies()
    {
        return _checkQuantifier(SpawnBlocker_Mode, SpawnBlocker.Select(_isBlocked).Count(), SpawnBlocker.Count)
            && _checkQuantifier(SpawnAllow_Mode, SpawnAllow.Select(n => !_isBlocked(n)).Count(), SpawnAllow.Count);
    }

    private static bool _checkQuantifier(Quantifiers mode, int count, int total)
    {
        return mode switch
        {
            Quantifiers.All => count == total,
            Quantifiers.Some => count > 0,
            Quantifiers.None => count == 0,
            _ => false,
        };
    }

    private bool _isBlocked(Node2D node)
    {
        return Block_Mode switch
        {
            BlockMode.Invisible => !node.Visible,
            _ => false,
        };
    }
}
