using Godot;
using Godot.Collections;
using System;

namespace breakout.customResources;

public static class LoadDefs
{
    public static Dictionary<string, T> LoadAll<[MustBeVariant] T>(ref Dictionary<string, T>? _allDefinitions, string ResourcePathFormat, Func<T, string> map) where T : class
    {
        if (_allDefinitions != null)
            return _allDefinitions;
        _allDefinitions = [];
        var resourceLoader = ResourceLoader.Singleton;
        foreach (var fileName in DirAccess.GetFilesAt(ResourcePathFormat))
        {
            var resourcePath = ResourcePathFormat + fileName;
            if (resourceLoader.Load(resourcePath) is T resource)
            {
                var id = map(resource);
                if (string.IsNullOrWhiteSpace(id))
                    continue;
                _allDefinitions[id] = resource;
            }
        }
        return _allDefinitions;
    }
}
