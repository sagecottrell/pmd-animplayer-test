using Godot;

namespace breakout.components;

public partial class DamageSource : GodotObject
{
    public int Amount;
    public object Source;
    public string DamageType;
}
