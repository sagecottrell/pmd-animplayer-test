using breakout.components.AIStrategies;
using breakout.customResources;
using Godot;

namespace breakout.components.scripts;

public partial class SquadFlag : Node2D
{
    [Export]
    public SquadStrategy? SquadStrategy { get; set; }

    public void SetTeam(TeamInfo? team)
    {
        var sprite = GetNode<Sprite2D>("FlagSprite");
        sprite.Modulate = team?.Color ?? Colors.White;
    }
}
