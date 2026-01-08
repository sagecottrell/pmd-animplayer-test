using Godot;

namespace breakout.components;

[GlobalClass]
public abstract partial class BaseModifier : Resource
{
    [Export]
    public int Priority;
}
