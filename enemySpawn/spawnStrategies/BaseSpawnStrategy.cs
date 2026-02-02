using breakout.customResources;
using Godot;
using System;

namespace breakout.enemySpawn.spawnStrategies;

public abstract partial class BaseSpawnStrategy : Node
{
    public abstract void DrawDebug(Node2D origin);

    public abstract bool CanSpawnEnemies(DateTime lastSpawnTime);

    public virtual void OnSpawned(WaveDefinition wave) { }
}
