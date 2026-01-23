using breakout.components.scripts;
using Godot;

namespace breakout.buildings;

public partial class BaseBuilding : Node2D
{
    [Export]
    public Sprite2D? Sprite;

    override public void _Ready()
    {
        if (this.TryGetComponent<SelectableComponent>(out var selectableComponent) &&
            this.TryGetComponent<UIComponent>(out var uiComponent))
        {
            selectableComponent.OnSelectionChanged += (v) =>
            {
                if (v)
                    uiComponent.ShowUI();
                else
                    uiComponent.RemoveUI();
            };
        }

        CallDeferred(nameof(setupShader));
    }

    void setupShader()
    {
        if (Sprite is Sprite2D s && s.Material is ShaderMaterial sm)
        {
            sm.SetShaderParameter("team_color", this.TryGetComponent<TeamComponent>(out var team) ? team.Team?.Color ?? Colors.Gray : Colors.Gray);
            sm.SetShaderParameter("replace_color", Color.Color8(128, 128, 128));
        }
    }
}
