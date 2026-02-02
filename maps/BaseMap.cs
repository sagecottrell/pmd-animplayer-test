using Godot;

[Tool]
public partial class BaseMap : Node2D
{
    [Export]
    public PackedScene? MainBaseScene { get; set; }
}
