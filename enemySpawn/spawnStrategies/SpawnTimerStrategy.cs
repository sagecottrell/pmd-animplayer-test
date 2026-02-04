using breakout.customResources;
using Godot;
using System;

namespace breakout.enemySpawn.spawnStrategies;

[Tool]
[GlobalClass]
public partial class SpawnTimerStrategy : BaseSpawnStrategy
{
    [Export]
    public float WaitTime { get; set; } = 10.0f;

    [Export]
    public double SpawnTimer { get; private set; }

    private DateTime _lastSpawnTime;

    public override void _EnterTree()
    {
        _lastSpawnTime = DateTime.Now - TimeSpan.FromSeconds(SpawnTimer);
    }

    public override bool CanSpawnEnemies()
    {
        return (DateTime.Now - _lastSpawnTime).TotalSeconds >= WaitTime;
    }

    public override void OnSpawned(WaveDefinition wave)
    {
        _lastSpawnTime = DateTime.Now;
        SpawnTimer = 0;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
            SpawnTimer = (DateTime.Now - _lastSpawnTime).TotalSeconds;
    }
}
