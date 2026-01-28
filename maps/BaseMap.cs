using Godot;
using System;

public partial class BaseMap : Node2D
{
    [Export]
    public PackedScene? MainBaseScene { get; set; }
}
