using Godot;

namespace breakout.customResources;

[Tool]
[GlobalClass]
public partial class TeamInfo : Resource
{
    [Export]
    public TeamIdEnum Id { get; set; }

    [Export]
    public Color Color { get; set; } = Colors.White;

    [Export]
    public string Name { get; set; } = "Default Team";
}
