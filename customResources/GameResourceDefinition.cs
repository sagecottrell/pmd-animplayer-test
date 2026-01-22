
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace breakout.customResources;

[GlobalClass]
[Tool]
public partial class GameResourceDefinition : Resource
{
    [Export]
    public GameResourceNames Name { get; set; } = GameResourceNames.None;

    [Export]
    public Texture2D? Icon { get; set; }

    static Dictionary<GameResourceNames, GameResourceDefinition>? _allDefinitions;
    public static Dictionary<GameResourceNames, GameResourceDefinition> AllDefinitions
    {
        get
        {
            if (_allDefinitions != null)
                return _allDefinitions;
            _allDefinitions = Enum.GetValues<GameResourceNames>().ToDictionary(v => v, v => new GameResourceDefinition());
            var resourceLoader = ResourceLoader.Singleton;
            foreach (var fileName in DirAccess.GetFilesAt("res://items/"))
            {
                if (fileName.EndsWith(".tres"))
                {
                    var resourcePath = "res://items/" + fileName;
                    if (resourceLoader.Load(resourcePath) is GameResourceDefinition resource && resource.Name != GameResourceNames.None)
                    {
                        _allDefinitions[resource.Name] = resource;
                    }
                }
            }
            return _allDefinitions;
        }
    }
}
