using breakout.customResources;
using Godot;
using System.Collections.Generic;

namespace breakout.buildings.UIs;

public partial class BasicUnitSpawnMenu : Control
{
    private PokeDefinition? _selectedUnit;

    [Signal]
    public delegate void SpawnUnitEventHandler(PokeDefinition poke);

    public override void _Ready()
    {
        GetNode<Button>("%SpawnUnit").Pressed += OnSpawn;
        var selector = GetNode<MenuButton>("%PokeSelector");
        var popup = selector.GetPopup();
        var resources = new Dictionary<long, PokeDefinition>();
        foreach (var (id, res) in PokeDefinition.AllDefinitions)
        {
            popup.AddIconItem(res.Sprite, res.Name, resources.Count);
            resources[resources.Count] = res;
        }
        popup.IdPressed += (id) => pick(resources[id]);
        pick(resources[0]);
    }

    public void pick(PokeDefinition? def)
    {
        if (_selectedUnit == def || def is null) return;
        var selector = GetNode<MenuButton>("%PokeSelector");
        selector.Icon = def.Sprite;
        _selectedUnit = def;
    }

    public void OnSpawn()
    {
        EmitSignalSpawnUnit(_selectedUnit);
    }

}
