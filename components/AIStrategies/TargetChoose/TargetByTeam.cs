using breakout.components.scripts;
using breakout.customResources;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace breakout.components.AIStrategies.TargetChoose;

public enum TargetMode
{
    Close,
    Strong,
}


[Tool]
[GlobalClass]
public partial class TargetByTeam : BaseTargetChooseStrategy
{
    [Export]
    public TeamIdEnum TargetTeams { get; set; }

    [Export]
    public TargetMode Mode { get; set; }

    [Export]
    public GroupNames Kind { get; set; }

    public override Node2D? GetTarget(Node2D unit)
    {
        var possibleTargets = new List<Node2D>();
        var teamNames = TargetTeams.ToString().Split(", ");
        var tree = unit.GetTree();
        var kinds = Kind.ByName().ToList();
        foreach (var team in teamNames)
        {
            foreach (var teamComponent in tree.GetNodesInGroup(team))
            {
                var node = teamComponent.GetParent();
                if (kinds.Any(x => node.IsInGroup(x)) && node is Node2D n2d)
                    possibleTargets.Add(n2d);
            }
        }
        return possibleTargets 
            .OrderBy<Node2D, float>(Mode switch
            {
                TargetMode.Close => x => (x.GlobalPosition - unit.GlobalPosition).LengthSquared(),
                TargetMode.Strong => x => x.TryGetComponent<HealthComponent>(out var h) ? h.CurrentMaxHealth : 0,
                _ => throw new NotImplementedException(),
            })
            .FirstOrDefault();
    }
}
