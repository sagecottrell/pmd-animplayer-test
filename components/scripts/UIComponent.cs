using breakout.resourceTypes;
using Godot;

namespace breakout.components.scripts;

public partial class UIComponent : Control, INodeComponent
{
    enum UIType
    {
        None,
        SameTeam,
        DifferentTeam,
        Neutral,
    }

    [Export]
    public PackedScene? SameTeam_Scene;
    [Export]
    public PackedScene? DifferentTeam_Scene;
    [Export]
    public PackedScene? Neutral_Scene;
    static UIComponent? currentDisplay;
    Node? ui;
    UIType currentType = UIType.None;

    public void ShowUI()
    {
        var scene = Neutral_Scene;
        var newType = UIType.Neutral;
        if (this.TryGetContext<IGameState>(out var gameState) &&
            gameState.PlayerTeam is TeamInfo playerTeam &&
            this.TryGetAncestorWithComponent<TeamComponent>(out var _, out var unitComponent) &&
            unitComponent.Team is TeamInfo unitTeam)
        {
            scene = unitTeam == playerTeam ? SameTeam_Scene : DifferentTeam_Scene;
            newType = unitTeam == playerTeam ? UIType.SameTeam : UIType.DifferentTeam;
        }

        if (scene is null || newType == currentType)
            return;
        currentDisplay?.HideUI();
        currentDisplay = this;
        currentType = newType;
        ui = scene.Instantiate();
        AddChild(ui);
    }

    public void HideUI()
    {
        currentType = UIType.None;
        ui?.QueueFree();
        ui = null;
    }

    public bool Displayed() => currentType != UIType.None;
}
