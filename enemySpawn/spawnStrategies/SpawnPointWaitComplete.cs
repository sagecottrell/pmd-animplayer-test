using Godot;
using System;

namespace breakout.enemySpawn.spawnStrategies;

[GlobalClass]
public partial class SpawnPointWaitComplete : BaseSpawnStrategy
{
    public SpawnPoint? SpawnPoint { get; set; }

    public override bool CanSpawnEnemies(DateTime lastSpawnTime) => SpawnPoint?.IsWaveSpawnComplete ?? true;

    public override void DrawDebug(Node2D origin)
    {
    }
}
