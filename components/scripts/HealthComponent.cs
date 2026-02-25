using Godot;
using System.Collections.Generic;

namespace breakout.components.scripts;

[GlobalClass]
public partial class HealthComponent : Node, INodeComponent
{
    [Signal]
    public delegate void OnDeathEventHandler(DamageSource source);

    [Signal]
    public delegate void OnHpChangeEventHandler(DamageSource source);

    [Export]
    public int MaxHealth = 100;

    public int CurrentMaxHealth { get; private set; } = 100;

    [Export]
    public int CurrentHealth = 100;

    public override void _Ready()
    {
        base._Ready();
        CurrentHealth = MaxHealth;
    }
    public void TakeDamage(DamageSource? amount)
    {
        if (amount is null)
            return;
        CurrentHealth -= amount.Amount;
        EmitSignalOnHpChange(amount);
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            EmitSignalOnDeath(amount);
        }
    }
    public void Heal(DamageSource amount)
    {
        CurrentHealth += amount.Amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }
    public int GetCurrentHealth()
    {
        return CurrentHealth;
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