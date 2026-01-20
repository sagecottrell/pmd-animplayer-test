using breakout.components.AIStrategies;
using breakout.components.scripts;
using breakout.resourceTypes;
using breakout.squad;
using Godot;
using Godot.Collections;
using System;

namespace breakout;

public partial class GameRoot : Node2D, IGameState
{
    [Export]
    public Array<Node2D> SpawnPoints = [];

    [Export]
    public TeamInfo? PlayerTeam { get; set; }

    public override void _Ready()
    {
        GetViewport().PhysicsObjectPickingSort = true;

        if (GetComponent.TryGetTeamComponent(GetNode("BaseBuilding"), out var bteam) && PlayerTeam is not null)
            bteam.SetTeam(PlayerTeam);

        var selectorOverlay = GetNode<SquadSelectorOverlay>(nameof(SquadSelectorOverlay));
        var squadContainer = GetNode<SquadContainer>(nameof(SquadContainer));
        selectorOverlay.OnSquadSelected += squadContainer.OnSquadSelected;
        selectorOverlay.OnSquadSetPosition += (s, p, t) =>
        {
            squadContainer.OnSquadSetPosition(s, p, t);
            squadContainer.OnSquadSelected(s);
        };

        if (GlobalSignals.Instance is not null)
        {
            GlobalSignals.Instance.OnUnitSpawn += (units) =>
            {
                foreach (var newNode in units)
                {
                    GetNode("%Units").AddChild(newNode);
                    newNode.Owner = this;
                }
            };
        }
        
    }


    public Dictionary<ResourceNames, long> Resources = [];

    public bool TryBuy(Dictionary<ResourceNames, long> values)
    {
        foreach (var kvp in values)
        {
            if (!Resources.TryGetValue(kvp.Key, out long value) || value < kvp.Value)
            {
                return false;
            }
        }
        foreach (var kvp in values)
        {
            Resources[kvp.Key] -= kvp.Value;
        }
        return true;
    }

    public bool GetResourceCount(ResourceNames r, out long value) => Resources.TryGetValue(r, out value);
}
