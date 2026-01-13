using breakout.resourceTypes;
using Godot;

namespace breakout.components.scripts;

public partial class SquadFlag : Node2D
{
    public void SetTeam(TeamInfo? team)
    {
        var sprite = GetNode<Sprite2D>("FlagSprite");
        sprite.Modulate = team?.Color ?? Colors.White;
    }
}
