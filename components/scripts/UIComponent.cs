using breakout.customResources;
using Godot;
using System.Collections.Generic;

namespace breakout.components.scripts;

[GlobalClass]
public partial class UIComponent : CanvasLayer, INodeComponent
{
    enum UIType
    {
        None,
        SameTeam,
        DifferentTeam,
        Neutral,
    }

    [Export]
    public Control? SameTeamUI;
    [Export]
    public Control? DifferentTeamUI;
    [Export]
    public Control? NeutralUI;

    [Export]
    /// if true, other UI components should restore this one when they hide themselves
    public bool OthersCanStack = false;

    static UIComponent? currentDisplay;
    static readonly Stack<UIComponent> displayStack = [];
    Control? ui;
    UIType currentType = UIType.None;

    public override void _Ready()
    {
        foreach (var child in GetChildren())
        {
            if (child == SameTeamUI || child == DifferentTeamUI || child == NeutralUI)
                RemoveChild(child);
        }
    }

    public void ShowUI()
    {
        var scene = NeutralUI;
        var newType = UIType.Neutral;
        if (this.TryGetContext<IGameState>(out var gameState) &&
            gameState.PlayerTeam is TeamInfo playerTeam &&
            this.TryGetAncestorWithComponent<TeamComponent>(out var _, out var unitComponent) &&
            unitComponent.Team is TeamInfo unitTeam)
        {
            scene = unitTeam == playerTeam ? SameTeamUI : DifferentTeamUI;
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
        ui ??= scene;
        AddChild(ui);
    }

    public void RemoveUI()
    {
        currentDisplay = null;
        currentType = UIType.None;
        RemoveChild(ui);
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
