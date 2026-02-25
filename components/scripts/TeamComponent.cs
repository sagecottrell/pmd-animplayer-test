using breakout.customResources;
using Godot;
using System;
using System.Collections.Generic;

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
            _addToGroups();
        }
    }

    public void SetTeam(TeamInfo? team)
    {
        Team = team;
    }

    private void _addToGroups()
    {
        if (!IsInsideTree())
            return;
        foreach (var group in GetGroups())
            this.RemoveFromGroupsAndSignal(group);
        if (Team?.Id is TeamIdEnum id)
        {
            foreach (var team in id.ToString().Split(", "))
                this.AddToGroupsAndSignal(team);
        }
    }

    public override void _EnterTree()
    {
        _addToGroups();
    }

    public bool EqTeam(TeamComponent? other, bool nullIsEq = false) => other?.Team?.Equals(Team) ?? (Team is null && nullIsEq);

    public override bool Equals(object? obj)
    {
        return obj is TeamComponent component &&
               EqualityComparer<TeamInfo?>.Default.Equals(team, component.team);
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(team);
        return hash.ToHashCode();
    }

    public static bool operator ==(TeamComponent? self, TeamComponent? other) => self?.EqTeam(other) ?? false;
    public static bool operator !=(TeamComponent? self, TeamComponent? other) => !(self == other);
}

public interface ITeamComponentModifier
{
}