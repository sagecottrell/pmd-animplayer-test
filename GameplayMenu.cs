using breakout.customResources;
using Godot;

namespace breakout;

public partial class GameplayMenu : Control
{

    public override void _Ready()
    {
        _setup_buildables();
    }

    void _setup_buildables()
    {
        var buildingsContainer = GetNode("%Buildings");
        foreach (var (id, def) in BuildableDefinition.AllDefinitions)
        {
            if (def.Name == BuildingNames.None || !def.Buildable)
                continue;

            var btn = new Button
            {
                Text = def.Name.ToString(),
                Icon = def.Icon,
            };
            buildingsContainer.AddChild(btn);
            btn.Pressed += () => GlobalSignals.Instance?.RequestBuildingCreate(def);
        }
    }
}
