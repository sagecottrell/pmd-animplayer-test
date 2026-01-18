using Godot;

namespace breakout.resourceTypes;

[Tool]
[GlobalClass]
public partial class UnitDefinition : Resource
{
    [Export]
    public AnimationLibrary? Sprites { get; set; }

    [Export]
    public string Name { get; set; } = "UnitDefaultName";

    [Export]
    public Texture2D? Icon { get; set; }
}
