using Godot;
using Godot.Collections;

namespace breakout.customResources;

[Tool]
[GlobalClass]
public partial class WaveDefinition : Resource
{
    [Export]
    public Array<WaveMember>? WaveMembers { get; set; }
}