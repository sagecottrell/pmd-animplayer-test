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

    [Export]
    public Node2D? UnitsContainer { get; private set; }

    [Export]
    public Node2D? BuildingsContainer { get; private set; }

    public override void _Ready()
    {
        GetViewport().PhysicsObjectPickingSort = true;

        var selectorOverlay = GetNode<SquadSelectorOverlay>(nameof(SquadSelectorOverlay));
        selectorOverlay.Enabled = true;
        var squadContainer = GetNode<SquadContainer>(nameof(SquadContainer));
        selectorOverlay.OnSquadSelected += squadContainer.OnSquadSelected;
        selectorOverlay.OnSquadSetPosition += (s, p, t) =>
        {
            squadContainer.OnSquadSetPosition(s, p, t);
            squadContainer.OnSquadSelected(s);
        };

        if (GlobalSignals.Instance is GlobalSignals signals)
        {
            signals.PlayerResourcesChange(_resources);
            signals.OnSquadCreate += squadContainer.OnNewSquad;
            signals.OnUnitSpawn += (units) =>
            {
                foreach (var newNode in units)
                {
                    UnitsContainer?.AddChild(newNode);
                    newNode.Owner = UnitsContainer?.GetOwner();
                }
            };
            signals.OnRequestBuildingCreate += (def) =>
            {
                selectorOverlay.Enabled = false;
                GetNode<BuildingPlacer>("%Buildings").StartPlacingBuilding(def);
            };
            signals.OnBuildingCreate += (building) =>
            {
                selectorOverlay.Enabled = true;
                building.Configure<TeamComponent>(teampCompt => teampCompt.SetTeam(PlayerTeam));
            };
            signals.OnSpawnAttack += _signals_OnSpawnAttack;
            signals.OnMoveFinish += _signals_OnMoveFinish;
        }

        if (GetNode("%TopUI").TryGetComponent<UIComponent>(out var component))
        {
            component.ShowUI();
        }

        if (FindChild("MainBase", false) is null && MapNode is BaseMap map && map.MainBaseScene is PackedScene mainBaseScene)
        {
            var b = mainBaseScene.Instantiate();
            AddChild(b);
            b.Configure<TeamComponent>(teamComp => teamComp.SetTeam(PlayerTeam));
        }
    }

    private void _signals_OnMoveFinish(Node2D moveNode)
    {
        moveNode.QueueFree();
    }

    private void _signals_OnSpawnAttack(MoveDefinition moveDefinition, Node2D caster)
    {
        // todo: pull node from pool to avoid allocation
        //GD.Print($"Spawning attack: {moveDefinition}");
        moveDefinition.SpawnMove(UnitsContainer, caster, null);
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
