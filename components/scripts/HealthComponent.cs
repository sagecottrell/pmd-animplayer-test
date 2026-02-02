using Godot;
using System.Collections.Generic;

namespace breakout.components.scripts;

[GlobalClass]
public partial class HealthComponent : Node, INodeComponent
{
    [Signal]
    public delegate void OnDeathEventHandler(DamageSource source);
    [Export]
    public int MaxHealth = 100;

    public int CurrentMaxHealth = 100;

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
            EmitSignalOnDeath(amount);
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

    public void Modify(IEnumerable<BaseModifier> modifiers)
    {
        foreach (var modifier in modifiers)
        {
            if (modifier is IHealthComponentModifier healthModifier)
            {
                // Apply health modifier logic here
            }
        }
    }
}

public interface IHealthComponentModifier
{

}