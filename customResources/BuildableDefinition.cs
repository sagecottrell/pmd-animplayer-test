using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace breakout.customResources;

[Tool]
[GlobalClass]
public partial class BuildableDefinition : Resource
{
    [Export]
    public BuildingNames Name { get; set; }

    [Export]
    public string Description { get; set; } = "";

    [Export]
    public Texture2D? Icon { get; set; }

    [Export]
    public PackedScene? BuildingScene { get; set; }

    const string DefinitionsPath = "res://buildings/definitions/";
    static Dictionary<BuildingNames, BuildableDefinition>? _allDefinitions;
    public static Dictionary<BuildingNames, BuildableDefinition> AllDefinitions
    {
        get
        {
            if (_allDefinitions != null)
                return _allDefinitions;
            _allDefinitions = Enum.GetValues<BuildingNames>().ToDictionary(v => v, v => new BuildableDefinition());
            var resourceLoader = ResourceLoader.Singleton;
            foreach (var fileName in DirAccess.GetFilesAt(DefinitionsPath))
            {
                var resourcePath = DefinitionsPath + fileName;
                var res = resourceLoader.Load(resourcePath);
                if (res is BuildableDefinition resource && resource.Name != BuildingNames.None)
                {
                    _allDefinitions[resource.Name] = resource;
                }
            }
            return _allDefinitions;
        }
    }
}
