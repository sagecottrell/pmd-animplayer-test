using breakout.components.AIStrategies;
using breakout.components.scripts;
using breakout.customResources;
using Godot;
using System;
using System.Collections.Generic;

namespace breakout.buildings.UIs;

public partial class BasicUnitSpawnMenu : Control
{
    private UnitDefinition? _selectedUnit;

    [Export]
    public PackedScene? SpawnScene;

    private SquadStrategy baseSquadStrat = new();

    public SquadRank squadRank = SquadRank.Frontline;

    public override void _Ready()
    {
        GetNode<Button>("%SpawnUnit").Pressed += OnSpawn;

        var selector = GetNode<MenuButton>("%PokeSelector");
        var popup = selector.GetPopup();
        var resources = new Dictionary<long, UnitDefinition>();
        foreach (var (id, res) in UnitDefinition.AllDefinitions)
        {
            popup.AddIconItem(res.Icon, res.Name, resources.Count);
            resources[resources.Count] = res;
        }
        popup.IdPressed += (id) => pick(resources[id]);
        pick(resources[0]);

        var rank = GetNode<OptionButton>("%RankSelector");
        rank.ItemSelected += RankSelected;
        rank.Select(0);
    }

    public void pick(UnitDefinition? def)
    {
        if (_selectedUnit == def || def is null) return;
        var selector = GetNode<MenuButton>("%PokeSelector");
        selector.Icon = def.Icon;
        _selectedUnit = def;
    }

    public void RankSelected(long idx)
    {
        var text = GetNode<OptionButton>("%RankSelector").GetItemText((int)idx);
        squadRank = Enum.Parse<SquadRank>(text);
    }

    public void OnSpawn()
    {
        if (SpawnScene == null || !this.TryGetContext<IGameState>(out var gameState) || !this.TryGetAncestorWithComponent<UnitSpawnerComponent>(out var building, out var spawnPoint))
            return;
        var newNode = SpawnScene.Instantiate<Node2D>();
        GlobalSignals.Instance?.UnitSpawn([newNode]);

        newNode.GlobalPosition = spawnPoint.GlobalPosition;
        if (GetComponent.TryGetTeamComponent(newNode, out var teamComponent) && gameState.PlayerTeam is not null)
            teamComponent.SetTeam(gameState.PlayerTeam);
        if (GetComponent.TryGetPmdSprite(newNode, out var sprite) && _selectedUnit is not null)
            sprite.Sprites = _selectedUnit.Sprites;
        if (GetComponent.TryGetSelectableComponent(newNode, out var selectable))
            selectable.UnselectedAlpha = 0.3f;
        if (GetComponent.TryGetAIComponent(newNode, out var aIComponent) && baseSquadStrat.Duplicate(deep: true) is SquadStrategy ss)
        {
            aIComponent.Strategy = ss;
            ss.SquadInfo.AddUnit(newNode, squadRank);
        }
    }

}
