using Godot;

namespace breakout.components;

[Tool]
[GlobalClass]
public abstract partial class BaseModifier : Resource
{
    [Export]
    public int Priority;
}
