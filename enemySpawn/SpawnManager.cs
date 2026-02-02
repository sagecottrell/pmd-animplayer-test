using breakout.customResources;
using breakout.enemySpawn.spawnStrategies;
using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace breakout.enemySpawn;

[Tool]
[GlobalClass]
public partial class SpawnManager : Node2D
{
    public Array<BaseSpawnStrategy> SpawnStrategies { get; set; } = [];

    [Signal]
    public delegate void OnSpawnEventHandler(WaveDefinition wave);

    [Export]
    public Array<WaveDefinition> Waves { get; set; } = [];

    [Export]
    public bool LoopWaves { get; set; } = true;

    [Export]
    public SpawnPoint? SpawnPoint { get; set; }

    public DateTime LastSpawnTime { get; private set; } = DateTime.MinValue;
    public int LastWaveIndex { get; private set; }

    public override void _Ready()
    {
        _getStrategyChildren();
    }

    public override void _Notification(int what)
    {
        switch ((long)what)
        {
            case NotificationChildOrderChanged: _getStrategyChildren(); break;
        }
    }

    private void _getStrategyChildren()
    {
        SpawnStrategies = [.. GetChildren().Where(x => x is BaseSpawnStrategy).Cast<BaseSpawnStrategy>()];
    }

    public override void _EnterTree()
    {
        if (!Engine.IsEditorHint()) return;
        SetNotifyLocalTransform(true);

        foreach (var strategy in SpawnStrategies)
        {
            if (strategy is SpawnPointWaitComplete sp)
            {
                sp.SpawnPoint = SpawnPoint;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            QueueRedraw();
        }
        else
        {
            TrySpawnWave();
        }
    }

    public override void _Draw()
    {
        if (!Engine.IsEditorHint()) return;
        if (SpawnStrategies.Count == 0) return;
        foreach (var strategy in SpawnStrategies)
        {
            strategy.DrawDebug(SpawnPoint ?? (Node2D)this);
        }
    }

    public bool CanSpawnWave() => SpawnStrategies.All(s => s.CanSpawnEnemies(LastSpawnTime));
    public async void TrySpawnWave()
    {
        if (SpawnPoint is null || !CanSpawnWave() || LastWaveIndex >= Waves.Count)
            return;
        var wave = Waves[LastWaveIndex++];
        if (LoopWaves && LastWaveIndex >= Waves.Count)
            LastWaveIndex = 0;
        await SpawnPoint.Spawn(wave);
        LastSpawnTime = DateTime.Now;
        EmitSignalOnSpawn(wave);
        return;
    }
}
