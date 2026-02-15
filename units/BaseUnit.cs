using breakout.components;
using breakout.components.AIStrategies;
using breakout.components.scripts;
using Godot;

namespace breakout.units;

[GlobalClass]
public partial class BaseUnit : RigidBody2D
{
    enum State
    {
        Idle,
        Walking,
        Attacking,
    }

    State state;

    StateMachine _stateMachine { get; init; }

    public BaseUnit()
    {
        _stateMachine = StateMachine.Create(new(){
            {Variant.From(State.Idle), Callable.From(on_idle)},
            {Variant.From(State.Walking), Callable.From(on_walking)},
            {Variant.From(State.Attacking), Callable.From(on_attacking)},
        });
    }

    void on_idle()
    {
        state = State.Idle;
        this.Configure<SpeedComponent>(s => s.Velocity = Vector2.Zero);
        this.Configure<PMDSprite>(p => p.Idle());
    }

    void on_walking()
    {
        state = State.Walking;
        this.Configure<PMDSprite>(p => p.Walk());
    }

    void on_attacking()
    {
        state = State.Attacking;
        this.Configure<SpeedComponent>(s => s.Velocity = Vector2.Zero);
        this.Configure<PMDSprite>(p => p.Attack());
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        if (this.TryGetComponent<SpeedComponent>(out var speed))
            state.LinearVelocity = speed.Velocity;
    }

    public override void _PhysicsProcess(double delta)
    {
        switch (state)
        {
            case State.Idle or State.Walking:
                {
                    if (this.TryGetComponent<SpeedComponent>(out var speed))
                        _stateMachine.Emit(speed.Velocity.IsZeroApprox() ? State.Idle : State.Walking);
                    break;
                }
        }
    }

    public override void _Ready()
    {
        CustomIntegrator = this.TryGetComponent<SpeedComponent>(out var _);
        state = State.Idle;
        this.Configure<PMDSprite, SpeedComponent>((p, speed) =>
        {
            p.OnHit += _on_hit;
            p.OnAnimFinish += _on_return;
        });
        this.Configure<HurtboxComponent>(h => h.OnHurt += _on_hurt);
        this.Configure<HealthComponent>(h => h.OnDeath += _on_death);
        this.Configure<AIComponent, SpeedComponent>((ai, s) =>
        {
            ai.OnNewTargetPoint += pt => _on_move(pt, s);
            ai.OnReachedTarget += _on_reach_target;
        });
        this.Configure<AggroArea, AIComponent>((a, ai) =>
        {
            a.AreaEntered += area => _on_aggro_area_entered(area, ai);
            a.AreaExited += area => _on_aggro_area_exited(area, ai);
        });
    }

    private void _on_aggro_area_entered(Area2D area, AIComponent ai)
    {
        if (area.GetParent() is BaseUnit unit)
        {
            if (unit.TryGetComponent<TeamComponent>(out var otherTeam) && this.TryGetComponent<TeamComponent>(out var myTeam) && myTeam == otherTeam)
            {
                ai.Strategy?.OnFriendlyNear(this, unit);
                return;
            }
            ai.Strategy?.OnEnemyNear(this, unit);
        }
    }

    private void _on_aggro_area_exited(Area2D area, AIComponent ai)
    {
        if (area.GetParent() is BaseUnit unit)
        {
            if (unit.TryGetComponent<TeamComponent>(out var otherTeam) && this.TryGetComponent<TeamComponent>(out var myTeam) && myTeam == otherTeam)
            {
                ai.Strategy?.OnFriendlyLeave(this, unit);
                return;
            }
            ai.Strategy?.OnEnemyLeave(this, unit);
        }
    }

    void _on_hit()
    {

    }

    void _on_death(DamageSource hurt)
    {

    }

    void _on_hurt(DamageSource hurt)
    {

    }

    void _on_return()
    {
        _stateMachine.Emit(this.TryGetComponent<SpeedComponent>(out var speed) && !speed.Velocity.IsZeroApprox() ? State.Walking : State.Idle);
    }

    void _on_attack()
    {
        switch (state)
        {
            case State.Idle or State.Walking:
                {
                    _stateMachine.Emit(State.Attacking);
                    this.Configure<PMDSprite>(p => p.Attack());
                    break;
                }
        }
    }

    void _on_shoot()
    {
        switch (state)
        {
            case State.Idle or State.Walking:
                {
                    _stateMachine.Emit(State.Attacking);
                    this.Configure<PMDSprite>(p => p.Shoot());
                    break;
                }
        }
    }

    void _on_charge()
    {
        switch (state)
        {
            case State.Idle or State.Walking:
                {
                    _stateMachine.Emit(State.Attacking);
                    this.Configure<PMDSprite>(p => p.Charge());
                    break;
                }
            case State.Attacking:
                _on_return();
                break;
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
        var dir = (targetPoint - GlobalPosition).Normalized();
        switch (state)
        {
            case State.Idle or State.Walking:
                {
                    if (targetPoint.DistanceTo(GlobalPosition) < speed.Speed * 2 / Engine.GetFramesPerSecond())
                    {
                        GlobalPosition = targetPoint;
                        _stateMachine.Emit(State.Idle);
                        speed.Velocity = Vector2.Zero;
                    }
                    else
                    {
                        dir = dir.Rotated(Mathf.Sin(Time.GetTicksMsec() / 1000) * 0.1f);
                        _stateMachine.Emit(State.Walking);
                        speed.Velocity = dir * speed.Speed;
                    }
                    this.Configure<PMDSprite>(p => p.Direction = dir);
                    break;
                }
        }
    }
}
