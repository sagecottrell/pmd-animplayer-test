using Godot;

namespace breakout.enemySpawn.spawnStrategies;

public abstract partial class BaseSpawnStrategy : Node
{
    public abstract void DrawDebug();

    public abstract bool CanSpawnEnemies();
}
