using breakout.customResources;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace breakout;

[Tool]
public partial class PlayerResourceDisplay : VBoxContainer
{
    [ExportToolButton("Refresh")]
    public Callable RefreshButton => new(this, nameof(refresh));

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
        GlobalSignals.Instance!.OnPlayerResourcesChange += _on_PlayerResourcesChange;
    }

    private void _on_PlayerResourcesChange(Godot.Collections.Dictionary<GameResourceNames, long> resources)
    {
        Resources = resources;
        _resourceDisplay.Update();
    }

    public void refresh()
    {
        _resourceDisplay.Update();
    }

}
