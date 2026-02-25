using breakout.components;
using breakout.components.AIStrategies;
using breakout.components.AIStrategies.TargetChoose;
using breakout.components.scripts;
using breakout.customResources;
using Godot;
using System.Threading.Tasks;

namespace breakout.enemySpawn;

[Tool]
[GlobalClass]
public partial class SpawnPoint : Node2D
{
    [Export]
    public PackedScene? SpawnScene;

    /// <summary>
    /// by default, uses the parent if this property is null
    /// </summary>
    [Export]
    public Node2D? TeamHolder;

    [Export]
    public AIStrategy AIStrategy = new SquadStrategy();

    [Signal]
    public delegate void OnWaveSpawnCompleteEventHandler(WaveDefinition wave);

    public bool IsWaveSpawnComplete { get; set; } = true;

    public Task Spawn(WaveDefinition wave)
    {
        if (!this.TryGetContext<IGameState>(out var ctx) || ctx.UnitsContainer is not Node2D unitsContainer)
            return Task.FromResult(0);
        return _spawnMembers(wave, unitsContainer);
    }

    private async Task _spawnMembers(WaveDefinition wave, Node2D unitsContainer)
    {
        var aiStrategy = AIStrategy.OnWaveSpawn();

        IsWaveSpawnComplete = false;
        // the state of this isn't saved to disk. if interrupted, the wave will be truncated
        foreach (var member in wave.WaveMembers ?? [])
        {
            if (member is null || member.PokeDefinition is null)
                continue;

            for (var i = 0; i < member.Quantity; i++)
            {
                var unitScene = SpawnScene?.Instantiate<Node2D>();
                if (unitScene == null)
                    continue;
                unitScene.GlobalPosition = GlobalPosition;
                member.PokeDefinition.ConfigureUnit(unitScene, member.Level);
                unitScene.Configure<TeamComponent>(t =>
                {
                    if ((TeamHolder ?? GetParent()).TryGetComponent<TeamComponent>(out var ti))
                        t.SetTeam(ti.Team);
                });
                unitsContainer.AddOwnedChild(unitScene);
                unitScene.Configure<AIComponent>(ai =>
                {
                    ai.Strategy = aiStrategy;
                });

                await this.GodotSleep(member.SpawnInterval);
            }
        }
        EmitSignalOnWaveSpawnComplete(wave);
        IsWaveSpawnComplete = true;
    }
}
