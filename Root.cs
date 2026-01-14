using breakout.components.scripts;
using breakout.resourceTypes;
using Godot;
using Godot.Collections;

namespace breakout;

public partial class Root : Node2D
{
    [Export]
    public Array<Node2D> SpawnPoints = [];
    [Export]
    public PackedScene? SpawnScene;

    [Export]
    public TeamInfo? PlayerTeam;


    public override void _Ready()
    {
        GetNode<Button>("%SpawnUnit").Pressed += OnSpawn;
        GetViewport().PhysicsObjectPickingSort = true;
        GetNode<SquadSelectorOverlay>(nameof(SquadSelectorOverlay)).PlayerTeam = PlayerTeam;
    }

    public void OnSpawn()
    {
        if (SpawnScene == null)
            return;
        var spawnPoint = SpawnPoints.PickRandom();
        var newNode = SpawnScene.Instantiate<Node2D>();
        newNode.GlobalPosition = spawnPoint.GlobalPosition;
        if (GetComponent.TryGetTeamComponent(newNode, out var teamComponent) && PlayerTeam is not null)
            teamComponent.SetTeam(PlayerTeam);
        GetNode("%Units").AddChild(newNode);
    }

}
