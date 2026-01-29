using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace breakout.customResources;

public enum PokeType
{
    [TypeInfo(color: "black")]
    None,
    [TypeInfo(color: "gray")]
    Normal,
    [TypeInfo(color: "red")]
    Fire,
    [TypeInfo(color: "blue")]
    Water,
    [TypeInfo(color: "yellow")]
    Electric,
    [TypeInfo(color: "green")]
    Grass,
    [TypeInfo(color: "#3dcef3")]
    Ice,
    [TypeInfo(color: "orange")]
    Fighting,
    [TypeInfo(color: "purple")]
    Poison,
    [TypeInfo(color: "brown")]
    Ground,
    [TypeInfo(color: "#81b9ef")]
    Flying,
    [TypeInfo(color: "#ef4179")]
    Psychic,
    [TypeInfo(color: "#91a119")]
    Bug,
    [TypeInfo(color: "#afa981")]
    Rock,
    [TypeInfo(color: "#704170")]
    Ghost,
    [TypeInfo(color: "#5060e1")]
    Dragon,
    [TypeInfo(color: "darkgrey")]
    Dark,
    [TypeInfo(color: "#60a1b8")]
    Steel,
    [TypeInfo(color: "pink")]
    Fairy,
}

[AttributeUsage(AttributeTargets.Field)]
public class TypeInfoAttribute(string color) : Attribute
{
    public Color Color { get; } = Color.FromString(color, Colors.Black);
}

public static class PokeTypeExtensions
{
    static readonly Dictionary<PokeType, Color> typeColorCache = [];
    public static Color GetColor(this PokeType type)
    {
        if (!typeColorCache.TryGetValue(type, out Color value))
        {
            var memberInfo = typeof(PokeType).GetMember(type.ToString()).First();
            var attribute = memberInfo.GetCustomAttribute<TypeInfoAttribute>();
            value = attribute?.Color ?? Colors.Black;
            typeColorCache[type] = value;
        }
        return value;
    }
}