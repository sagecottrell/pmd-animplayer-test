using breakout.customResources;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace breakout;

public partial class PlayerResourceDisplay : VBoxContainer
{
    readonly For<KeyValuePair<GameResourceNames, long>, VBoxContainer> _resourceDisplay = new()
    {
        Key = kvp => kvp.Key.ToString(),
        Map = kvp =>
        {
            var icon = GameResourceDefinition.AllDefinitions[kvp.Key].Icon;
            var panel = new PanelContainer()
            {
                SizeFlagsVertical = SizeFlags.ExpandFill,
            };
            var hBox = new HBoxContainer();
            if (icon is not null)
            {
                var textureRect = new TextureRect
                {
                    Texture = icon,
                    SizeFlagsVertical = SizeFlags.ShrinkCenter,
                };
                hBox.AddChild(textureRect);
            }
            var label = new Label
            {
                Text = $"{kvp.Value}",
            };
            hBox.AddChild(label);
            panel.AddChild(hBox);
            return (panel, x => label.Text = $"{x.Value}");
        }
    };

    [Export]
    public Godot.Collections.Dictionary<GameResourceNames, long> Resources { get; set; } = [];

    public override void _Ready()
    {
        _resourceDisplay.Container = this;
        _resourceDisplay.Items = () => Resources.OrderBy(x => x.Key);
        _resourceDisplay.Update();
        if (GlobalSignals.Instance is not null)
        {
            GlobalSignals.Instance.OnPlayerResourcesChange += _on_PlayerResourcesChange;
        }
        
    }

    private void _on_PlayerResourcesChange(Godot.Collections.Dictionary<GameResourceNames, long> resources)
    {
        Resources = resources;
        _resourceDisplay.Update();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _resourceDisplay.Dispose();
    }
}
