using Godot;

namespace breakout.components;

[GlobalClass]
public partial class DamageSource : GodotObject
{
    public int Amount;
    public Node2D? Source;
    public string? DamageType;
}
