using breakout.customResources;
using Godot;
using System;

namespace breakout.enemySpawn.spawnStrategies;

public abstract partial class BaseSpawnStrategy : Node
{
    public virtual void DrawDebug(Node2D origin) { }

    public abstract bool CanSpawnEnemies();

    public virtual void OnSpawned(WaveDefinition wave) { }
}
