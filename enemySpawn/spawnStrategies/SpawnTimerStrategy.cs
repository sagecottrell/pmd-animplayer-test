using breakout.customResources;
using Godot;
using System;

namespace breakout.enemySpawn.spawnStrategies;

[Tool]
[GlobalClass]
public partial class SpawnTimerStrategy : BaseSpawnStrategy
{
    [Export]
    public float Timer { get; set; } = 10.0f;

    public override bool CanSpawnEnemies(DateTime lastSpawnTime)
    {
        return (DateTime.Now - lastSpawnTime).TotalSeconds >= Timer;
    }

    public override void DrawDebug(Node2D origin)
    {
    }
}
