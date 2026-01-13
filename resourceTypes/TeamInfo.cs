using Godot;

namespace breakout.resourceTypes;

[GlobalClass]
public partial class TeamInfo : Resource
{
    [Export]
    public int Id { get; set; }

    [Export]
    public Color Color { get; set; } = Colors.White;

    [Export]
    public string Name { get; set; } = "Default Team";
}
