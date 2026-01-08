using Godot;

namespace breakout.components;

[GlobalClass]
public partial class UserInputComponent : BaseComponent
{
    [Signal]
    public delegate void OnMovementEventHandler(Vector2 direction);

    [Signal]
    public delegate void OnAttackEventHandler();
    [Signal]
    public delegate void OnShootEventHandler();
    [Signal]
    public delegate void OnChargeEventHandler();

    public Vector2 Direction;
    public override void _UnhandledInput(InputEvent @event)
    {
        var new_dir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (!new_dir.IsEqualApprox(Direction))
        {
            Direction = new_dir;
            EmitSignalOnMovement(Direction);
        }
        if (@event.IsActionPressed("attack"))
        {
            EmitSignalOnAttack();
        }
        if (@event.IsActionPressed("shoot"))
        {
            EmitSignalOnShoot();
        }
        if (@event.IsActionPressed("charge"))
        {
            EmitSignalOnCharge();
        }
    }

}
