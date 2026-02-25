using Godot;

namespace breakout.components.scripts;

[GlobalClass]
public partial class HpBar : HBoxContainer, INodeComponent
{
    [Export]
    public HealthComponent? Health { get; set; }

    [Export]
    public bool HideIfFull { get; set; }

    private ProgressBar? _progressBar;

    public override void _Ready()
    {
        _progressBar = GetNode<ProgressBar>("progress");

        HpChange(null);
    }

    public void HpChange(DamageSource? damageSource)
    {
        if (_progressBar is null || Health is null)
            return;
        _progressBar.MaxValue = Health.CurrentMaxHealth;
        _progressBar.Value = Health.CurrentHealth;

        Visible = !(HideIfFull && Health.CurrentHealth == Health.CurrentMaxHealth);
    }

}
