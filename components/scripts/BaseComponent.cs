using Godot;
using System.Collections.Generic;

namespace breakout.components;

[GlobalClass]
public abstract partial class BaseComponent : Node
{
    public virtual void Modify(IEnumerable<BaseModifier> modifiers) { }
}
