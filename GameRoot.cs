using breakout.buildings;
using breakout.components.scripts;
using breakout.customResources;
using breakout.squad;
using Godot;
using Godot.Collections;

namespace breakout;

public partial class GameRoot : Node2D, IGameState
{
    [Export]
    public TeamInfo? PlayerTeam { get; set; }

    [Export]
    public BaseMap? MapNode { get; set; }

    public override void _Ready()
    {
        GetViewport().PhysicsObjectPickingSort = true;
        GlobalSignals.Instance?.PlayerResourcesChange(_resources);

        var selectorOverlay = GetNode<SquadSelectorOverlay>(nameof(SquadSelectorOverlay));
        selectorOverlay.Enabled = true;
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
            GlobalSignals.Instance.OnRequestBuildingCreate += (def) =>
            {
                selectorOverlay.Enabled = false;
                GetNode<BuildingPlacer>("%Buildings").StartPlacingBuilding(def);
            };
            GlobalSignals.Instance.OnBuildingCreate += (building) =>
            {
                selectorOverlay.Enabled = true;
                if (building.TryGetComponent<TeamComponent>(out var teamComp) && PlayerTeam is not null)
                {
                    teamComp.SetTeam(PlayerTeam);
                }
            };
        }

        if (GetNode("%TopUI").TryGetComponent<UIComponent>(out var component))
        {
            component.ShowUI();
        }

        if (FindChild("MainBase", false) is null && MapNode is BaseMap map && map.MainBaseScene is PackedScene mainBaseScene)
        {
            var b = mainBaseScene.Instantiate();
            AddChild(b);
            if (b.TryGetComponent<TeamComponent>(out var teamComp) && PlayerTeam is not null)
            {
                teamComp.SetTeam(PlayerTeam);
            }
        }
    }

    [Export]
    public Dictionary<GameResourceNames, long> Resources { get => _resources; 
        set 
        { 
            _resources = value; 
            GlobalSignals.Instance?.PlayerResourcesChange(_resources);
        }
    }
    Dictionary<GameResourceNames, long> _resources = [];

    public bool TryBuy(System.Collections.Generic.IDictionary<GameResourceNames, long> values)
    {
        foreach (var kvp in values)
        {
            if (!_resources.TryGetValue(kvp.Key, out long value) || value < kvp.Value)
            {
                return false;
            }
        }
        foreach (var kvp in values)
        {
            _resources[kvp.Key] -= kvp.Value;
        }
        GlobalSignals.Instance?.PlayerResourcesChange(_resources);
        return true;
    }

    public bool GetResourceCount(GameResourceNames r, out long value) => _resources.TryGetValue(r, out value);
}
