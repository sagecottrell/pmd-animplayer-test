using breakout.customResources;
using Godot;

namespace breakout.components.scripts;

[GlobalClass]
public partial class AttackSpawnComponent : Node2D, INodeComponent
{
    [Signal]
    public delegate void OnAttackQueuedEventHandler(MoveDefinition move);

    public MoveDefinition? QueuedAttack { get; set; }

    public void SpawnAttack(MoveDefinition move)
    {
        QueuedAttack = move;
        EmitSignalOnAttackQueued(move);
    }
}
