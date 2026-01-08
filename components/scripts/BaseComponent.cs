using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace breakout.components;

[GlobalClass]
public abstract partial class BaseComponent: Node
{
    private readonly static Dictionary<Type, string[]> _modifiers = [];
    private readonly static Dictionary<Type, string> _names = [];

    public virtual void Modify(IEnumerable<BaseModifier> modifiers) { }

    public bool IsModifierForThisComponent(BaseModifier modifier)
    {
        if (!_names.TryGetValue(GetType(), out var componentName))
            _names[GetType()] = componentName = GetType().Name.TrimSuffix("Component");
        if (!_modifiers.TryGetValue(modifier.GetType(), out var methods))
        {
            _modifiers[modifier.GetType()] = methods = modifier.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(m => m.Name)
                .Where(m => m.StartsWith("Modify"))
                .Select(m => m.TrimPrefix("Modify"))
                .ToArray();
        }
        return methods.Contains(componentName);
    }
}
