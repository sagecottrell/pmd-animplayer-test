using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace breakout;

public static class EnumExtensions
{
    private static readonly Dictionary<Type, Dictionary<object, IImmutableSet<StringName>>> cache = [];

    public static IImmutableSet<StringName> ByStringName<T>(this T enumValue) where T : Enum
    {
        if (!cache.TryGetValue(typeof(T), out var typeCache))
        {
            typeCache = [];
            cache[typeof(T)] = typeCache;
        }
        if (!typeCache.TryGetValue(enumValue, out var hashSet))
        {
            hashSet = [.. enumValue.ToString().Split(", ").Select(x => new StringName(x))];
            typeCache[enumValue] = hashSet;
        }
        return hashSet;
    }
}
