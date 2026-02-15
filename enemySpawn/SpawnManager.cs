using breakout.customResources;
using breakout.enemySpawn.spawnStrategies;
using Godot;
using Godot.Collections;
using System;
using System.Linq;

namespace breakout.enemySpawn;

[Tool]
[GlobalClass]
public partial class SpawnManager : Node2D
{
    public Array<BaseSpawnStrategy> SpawnStrategies { get; set; } = [];

    [Signal]
    public delegate void OnSpawnEventHandler(WaveDefinition wave);

    [Export]
    public int LastWaveIndex { get; private set; }

    [Export]
    public Array<WaveDefinition> Waves { get; set; } = [];

    [Export]
    public Array<WaveDefinition> LoopWaves { get; set; } = [];

    [Export]
    public SpawnPoint? SpawnPoint { get; set; }

    [Export]
    public bool Enabled { get; set; } = true;

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

    public bool CanSpawnWave() => SpawnStrategies.All(s => s.CanSpawnEnemies());
    public async void TrySpawnWave()
    {
        if (!Enabled || SpawnPoint is null || !CanSpawnWave() || LastWaveIndex >= LoopWaves.Count + Waves.Count)
            return;
        var wave = LastWaveIndex < Waves.Count ? Waves[LastWaveIndex] : LoopWaves[LastWaveIndex - Waves.Count];
        LastWaveIndex++;
        if (LastWaveIndex >= LoopWaves.Count + Waves.Count)
            LastWaveIndex = Waves.Count;
        await SpawnPoint.Spawn(wave);
        foreach (var strategy in SpawnStrategies)
            strategy.OnSpawned(wave);
        EmitSignalOnSpawn(wave);
        return;
    }
}
