using breakout.resourceTypes;
using Godot;

namespace breakout.components;

[GlobalClass]
public partial class TeamComponent : Node
{
    TeamInfo? team;
    [Export]
    public TeamInfo? Team
    {
        get => team; private set
        {
            if (value is null)
                return;
            team = value;
        }
    }

    public void SetTeam(TeamInfo team)
    {
        Team = team;
    }

}

public interface ITeamComponentModifier
{
}