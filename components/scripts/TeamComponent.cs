using Godot;
using System;

namespace breakout.components;

public partial class TeamComponent : BaseComponent
{
    [Export]
    public int TeamId { get; private set; }

    public void ClearTeam()
    {
        TeamId = -1;
    }

    public void SetTeam(int teamId)
    {
        if (teamId < 0)
            throw new ArgumentException("Team ID must be non-negative.", nameof(teamId));
        TeamId = teamId;
    }

}
