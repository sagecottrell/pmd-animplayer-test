using breakout.components.AIStrategies;
using breakout.customResources;
using Godot;

namespace breakout.components.scripts;

[GlobalClass]
public partial class UnitSpawnerComponent : Node2D, INodeComponent
{
    [Export]
    public PackedScene? SpawnScene;

    [Export]
    public SquadStrategy? BaseSquadStrategy;

    public override void _Ready()
    {
        BaseSquadStrategy.Target ??= this.TryGetContext<Node2D>(out var n2d) ? n2d : null;
        GlobalSignals.Instance?.SquadCreate(BaseSquadStrategy);
    }

    public void SpawnUnit(PokeDefinition pokeDefinition)
    {
        if (SpawnScene == null || !this.TryGetContext<IGameState>(out var gameState) || !this.TryGetAncestorWithComponent<UnitSpawnerComponent>(out var building, out var spawnPoint))
            return;
        var newNode = SpawnScene.Instantiate<Node2D>();
        GlobalSignals.Instance?.UnitSpawn([newNode]);
        newNode.Name = pokeDefinition.Name ?? "unit";

        newNode.GlobalPosition = spawnPoint.GlobalPosition;
        pokeDefinition.ConfigureUnit(newNode, 1);
        if (gameState.PlayerTeam is not null)
            newNode.Configure<TeamComponent>(team => team.SetTeam(gameState.PlayerTeam));
        newNode.Configure<SelectableComponent>(selectable => selectable.UnselectedAlpha = 0.3f);
        newNode.Configure<AIComponent>(ai =>
        {
            ai.Strategy = BaseSquadStrategy;
            BaseSquadStrategy?.AddUnit(newNode);
        });
        newNode.Configure<UnitClickAreaComponent>(click => click.InputPickable = true);
    }
}
