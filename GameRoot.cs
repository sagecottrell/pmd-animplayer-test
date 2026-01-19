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
    public PackedScene? SpawnScene;

    [Export]
    public TeamInfo? PlayerTeam { get; set; }

    private UnitDefinition? _selectedUnit;

    private SquadStrategy baseSquadStrat = new();

    public SquadRank squadRank = SquadRank.Frontline;

    public override void _Ready()
    {
        GetViewport().PhysicsObjectPickingSort = true;

        GetNode<Button>("%SpawnUnit").Pressed += OnSpawn;

        if (GetComponent.TryGetTeamComponent(GetNode("BaseBuilding"), out var bteam) && PlayerTeam is not null)
            bteam.SetTeam(PlayerTeam);

        foreach (var value in Enum.GetValues<SquadRank>())
        {
            var node = GetNode<Button>($"%{Enum.GetName(value)}");
            node.ButtonPressed = squadRank == value;
            node.Pressed += () =>
            {
                squadRank = value;
            };
        }

        var selectorOverlay = GetNode<SquadSelectorOverlay>(nameof(SquadSelectorOverlay));
        var squadContainer = GetNode<SquadContainer>(nameof(SquadContainer));
        selectorOverlay.OnSquadSelected += squadContainer.OnSquadSelected;
        selectorOverlay.OnSquadSetPosition += (s, p, t) =>
        {
            squadContainer.OnSquadSetPosition(s, p, t);
            squadContainer.OnSquadSelected(s);
        };

        var selector = GetNode<MenuButton>("%PokeSelector");
        var popup = selector.GetPopup();
        var resources = new Dictionary<long, UnitDefinition>();
        foreach (var file in DirAccess.Open("res://units/types/").GetFiles())
        {
            var res = ResourceLoader.Load<UnitDefinition>($"res://units/types/{file}");
            popup.AddIconItem(res.Icon, res.Name, resources.Count);
            resources[resources.Count] = res;
        }
        popup.IdPressed += (id) => pick(resources[id]);
        pick(resources[0]);
    }

    public void pick(UnitDefinition? def)
    {
        if (_selectedUnit == def || def is null) return;
        var selector = GetNode<MenuButton>("%PokeSelector");
        selector.Icon = def.Icon;
        _selectedUnit = def;
    }

    public void OnSpawn()
    {
        if (SpawnScene == null)
            return;
        var spawnPoint = SpawnPoints.PickRandom();
        var newNode = SpawnScene.Instantiate<Node2D>();
        newNode.GlobalPosition = spawnPoint.GlobalPosition;
        if (GetComponent.TryGetTeamComponent(newNode, out var teamComponent) && PlayerTeam is not null)
            teamComponent.SetTeam(PlayerTeam);
        if (GetComponent.TryGetPmdSprite(newNode, out var sprite) && _selectedUnit is not null)
            sprite.Sprites = _selectedUnit.Sprites;
        if (GetComponent.TryGetSelectableComponent(newNode, out var selectable))
            selectable.UnselectedAlpha = 0.3f;
        GetNode("%Units").AddChild(newNode);
        newNode.Owner = this;
        if (GetComponent.TryGetAIComponent(newNode, out var aIComponent) && baseSquadStrat.Duplicate(deep: true) is SquadStrategy ss)
        {
            aIComponent.Strategy = ss;
            ss.SquadInfo.AddUnit(newNode, squadRank);
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
