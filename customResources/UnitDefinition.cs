using Godot;
using System.Collections.Generic;

namespace breakout.customResources;

[Tool]
[GlobalClass]
public partial class UnitDefinition : Resource
{
    [Export]
    public AnimationLibrary? Sprites { get; set; }

    [Export]
    public string Name { get; set; } = "";

    [Export]
    public Texture2D? Icon { get; set; }

    const string ResourcePathFormat = "res://units/types/";
    static Dictionary<string, UnitDefinition>? _allDefinitions;
    public static Dictionary<string, UnitDefinition> AllDefinitions
    {
        get
        {
            if (_allDefinitions != null)
                return _allDefinitions;
            _allDefinitions = [];
            var resourceLoader = ResourceLoader.Singleton;
            foreach (var fileName in DirAccess.GetFilesAt(ResourcePathFormat))
            {
                var resourcePath = ResourcePathFormat + fileName;
                if (resourceLoader.Load(resourcePath) is UnitDefinition resource && !string.IsNullOrWhiteSpace(resource.Name))
                {
                    _allDefinitions[resource.Name] = resource;
                }
            }
            return _allDefinitions;
        }
    }
}
