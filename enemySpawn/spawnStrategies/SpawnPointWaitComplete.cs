using Godot;

namespace breakout.enemySpawn.spawnStrategies;

[GlobalClass]
public partial class SpawnPointWaitComplete : BaseSpawnStrategy
{
    [Export]
    public SpawnPoint? SpawnPoint { get; set; }

    public override void _Ready()
    {
        if (SpawnPoint is null && GetParent() is SpawnManager spawnManager)
            SpawnPoint = spawnManager.SpawnPoint;
    }

    public override bool CanSpawnEnemies() => SpawnPoint?.IsWaveSpawnComplete ?? true;
}
