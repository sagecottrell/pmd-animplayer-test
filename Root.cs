using breakout.components.scripts;
using breakout.resourceTypes;
using breakout.squad;
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
        GetViewport().PhysicsObjectPickingSort = true;

        GetNode<Button>("%SpawnUnit").Pressed += OnSpawn;

        var selectorOverlay = GetNode<SquadSelectorOverlay>(nameof(SquadSelectorOverlay));
        selectorOverlay.PlayerTeam = PlayerTeam;

        var squadContainer = GetNode<SquadContainer>(nameof(SquadContainer));
        selectorOverlay.OnSquadSelected += squadContainer.OnSquadSelected;
        selectorOverlay.OnSquadSetPosition += (s, p, t) =>
        {
            squadContainer.OnSquadSetPosition(s, p, t);
            squadContainer.OnSquadSelected(s);
        };
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
        newNode.Owner = this;
    }

}
