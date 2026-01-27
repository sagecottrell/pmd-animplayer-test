using breakout.enemySpawn.spawnStrategies;
using Godot;

namespace breakout.enemySpawn;

[Tool]
public partial class BaseEnemySpawner : Node2D
{
    [Export]
    public BaseSpawnStrategy? SpawnStrategy { get; set; }

    public override void _EnterTree()
    {
        if (!Engine.IsEditorHint()) return;
        SetNotifyLocalTransform(true);
    }

    public override void _Process(double delta)
    {
        if (!Engine.IsEditorHint()) return;
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (!Engine.IsEditorHint()) return;
        if (SpawnStrategy is null) return;
        if (SpawnStrategy.GetParent() == this)
            SpawnStrategy.DrawDebug();
    }
}
