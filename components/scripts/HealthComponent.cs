using Godot;

namespace breakout.components;

public partial class HealthComponent: BaseComponent
{
    [Signal]
    public delegate void OnDeathEventHandler();
    [Export]
    public int MaxHealth = 100;
    private int currentHealth;
    public override void _Ready()
    {
        base._Ready();
        currentHealth = MaxHealth;
    }
    public void TakeDamage(DamageSource amount)
    {
        currentHealth -= amount.Amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            EmitSignalOnDeath();
        }
    }
    public void Heal(DamageSource amount)
    {
        currentHealth += amount.Amount;
        if (currentHealth > MaxHealth)
        {
            currentHealth = MaxHealth;
        }
    }
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
