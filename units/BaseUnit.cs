using breakout.components;
using breakout.components.AIStrategies;
using breakout.components.scripts;
using breakout.customResources;
using Godot;

namespace breakout.units;

[GlobalClass]
public partial class BaseUnit : RigidBody2D
{
    [Export]
    public PokeDefinition? PokeDefinition
    {
        get => pokeDefinition; set
        {
            pokeDefinition = value;
            if (value is not null && IsInsideTree())
                _setPokeDef(value);
        }
    }

    private PokeDefinition? pokeDefinition;
    bool _canMove = true;

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        if (this.TryGetComponent<SpeedComponent>(out var speed) && _canMove)
            state.LinearVelocity = speed.Velocity;
        else
            state.LinearVelocity = Vector2.Zero;
    }

    private void _setPokeDef(PokeDefinition pokeDef)
    {
        this.Configure<PMDSprite>(p => p.Sprites = pokeDef.AnimationLibrary);
        if (this.TryGetComponent<TeamComponent>(out var team))
        {
            Name = $"{team.Name}@{pokeDef.Name}";
        }
    }

    public override void _Ready()
    {
        CustomIntegrator = this.TryGetComponent<SpeedComponent>(out var _);
        if (PokeDefinition is not null)
            _setPokeDef(PokeDefinition);
        this.Configure<PMDSprite, SpeedComponent>((p, speed) =>
        {
            p.OnHit += _on_hit;
            p.OnAnimFinish += _on_return;
        });
        this.Configure<HurtboxComponent, HealthComponent>((hit, hp) =>
        {
            hit.OnHurt += hp.TakeDamage;
        });
        this.Configure<HealthComponent, HpBar>((health, bar) =>
        {
            health.OnDeath += _on_death;
            health.OnHpChange += bar.HpChange;
        });
        this.Configure<AIComponent, SpeedComponent>((ai, s) =>
        {
            ai.OnNewTargetPoint += pt => _on_move(pt, s);
            ai.OnReachedTarget += _on_reach_target;
        });
        this.Configure<AggroArea, AIComponent>((a, ai) =>
        {
            a.OnFriendlyEnter += unit => ai.Strategy?.OnFriendlyNear(this, unit);
            a.OnFriendlyExit += unit => ai.Strategy?.OnFriendlyLeave(this, unit);
            a.OnEnemyEnter += unit => ai.Strategy?.OnEnemyNear(this, unit);
            a.OnEnemyExit += unit => ai.Strategy?.OnEnemyLeave(this, unit);
        });
        this.Configure<AttackSpawnComponent>(a => a.OnAttackQueued += _on_attack_queued);
    }

    private void _on_hit()
    {
        if (this.TryGetComponent<AttackSpawnComponent>(out var attackSpawn) && attackSpawn.QueuedAttack is not null)
        {
            GlobalSignals.Instance?.SpawnAttack(attackSpawn.QueuedAttack, this);
            attackSpawn.QueuedAttack = null;
        }
    }

    void _on_death(DamageSource hurt)
    {

    }

    void _on_return()
    {
        _canMove = true;
    }

    private void _on_attack_queued(MoveDefinition move)
    {
        if (this.TryGetComponent<PMDSprite>(out var sprite))
        {
            sprite.Play(string.IsNullOrWhiteSpace(move.AnimationToPlay) ? "Attack" : move.AnimationToPlay);
            _canMove = false;
        }
    }

    void _on_reach_target(Vector2 globalPosition)
    {
        if (this.TryGetComponent<AIComponent>(out var ai) && ai.Strategy is SquadStrategy ss)
            this.Configure<PMDSprite>(p => p.Direction = ss.FacingDirection);
        GlobalPosition = globalPosition;
    }

    void _on_move(Vector2 targetPoint, SpeedComponent speed)
    {
        if (!_canMove)
            return;
        var dir = (targetPoint - GlobalPosition).Normalized();
        if (targetPoint.DistanceTo(GlobalPosition) < speed.Speed * 2 / Engine.GetFramesPerSecond())
        {
            GlobalPosition = targetPoint;
            speed.Velocity = Vector2.Zero;
            this.Configure<PMDSprite>(p =>
            {
                p.Idle();
            });
        }
        else
        {
            dir = dir.Rotated(Mathf.Sin(Time.GetTicksMsec() / 1000) * 0.1f);
            speed.Velocity = dir * speed.Speed;
            this.Configure<PMDSprite>(p =>
            {
                p.Direction = dir;
                p.Walk();
            });
        }
    }
}
