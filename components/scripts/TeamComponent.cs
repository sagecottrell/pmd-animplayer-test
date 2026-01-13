using breakout.resourceTypes;
using Godot;
using System;

namespace breakout.components;

[GlobalClass]
public partial class TeamComponent : Node
{
    TeamInfo? team;
    [Export]
    public TeamInfo? Team { get => team; private set
        {
            if (value is null)
                return;
            team = value;
        }
    }

    public void SetTeam(TeamInfo team)
    {
        if (team is null)
            throw new ArgumentException($"Team must not be null", nameof(team));
        Team = team;
    }

}

public interface ITeamComponentModifier
{
}