using breakout.resourceTypes;
using Godot;

namespace breakout.components.scripts;

[GlobalClass]
public partial class TeamComponent : Node, INodeComponent
{
    [Signal]
    public delegate void OnTeamChangedEventHandler(TeamInfo newTeam);

    TeamInfo? team;
    [Export]
    public TeamInfo? Team
    {
        get => team; private set
        {
            if (value is null || team == value)
                return;
            team = value;
            EmitSignalOnTeamChanged(value);
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