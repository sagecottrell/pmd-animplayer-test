using breakout.components.scripts;
using Godot;

namespace breakout.buildings;

public partial class BaseBuilding : Node2D
{
    [Export]
    public Vector2 SpawnPointRelative;

    override public void _Ready()
    {
        this.TryGetComponent<SelectableComponent>(out var selectableComponent);
        this.TryGetComponent<UIComponent>(out var uiComponent);

        selectableComponent.OnSelectionChanged += (v) =>
        {
            if (v)
                uiComponent.ShowUI();
            else
                uiComponent.HideUI();
        };
    }
}
