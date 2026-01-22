using breakout.customResources;
using Godot;
using System.Collections.Generic;

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

    [Export]
    /// if true, other UI components should restore this one when they hide themselves
    public bool OthersCanStack = false;

    static UIComponent? currentDisplay;
    static readonly Stack<UIComponent> displayStack = [];
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
        if (currentDisplay is UIComponent c) {
            if (c.OthersCanStack) c.HideStackableUI(); 
            else c.RemoveUI(); 
        }
        currentDisplay = this;
        currentType = newType;
        ui ??= scene.Instantiate();
        AddChild(ui);
    }

    public void RemoveUI()
    {
        currentDisplay = null;
        currentType = UIType.None;
        ui?.QueueFree();
        ui = null;

        if (displayStack.TryPop(out var nextDisplay))
        {
            nextDisplay.ShowUI();
        }
    }

    public void HideStackableUI()
    {
        displayStack.Push(this);
        currentType = UIType.None;
        RemoveChild(ui);
    }

    public bool Displayed() => currentType != UIType.None;
}
