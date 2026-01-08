using Godot;

namespace breakout.components;

[GlobalClass]
public partial class DamageSource : GodotObject
{
    public int Amount;
    public object? Source;
    public string? DamageType;
}
