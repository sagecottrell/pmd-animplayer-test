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
        this.Configure<PMDSprite>(p =>
        {
            p.OnHit += on_hit;
            p.OnAnimFinish += on_return;
        });
        this.Configure<HurtboxComponent>(h => h.OnHurt += on_hurt);
        this.Configure<HealthComponent>(h => h.OnDeath += on_death);
        this.Configure<AIComponent>(ai =>
        {
            ai.OnNewVelocity += on_move;
            ai.OnReachedTarget += on_reach_target;
        });
    }

    void on_hit()
    {

    }

    void on_death(DamageSource hurt)
    {

    }

    void on_hurt(DamageSource hurt)
    {

    }

    void on_return()
    {
        if (this.TryGetComponent<SpeedComponent>(out var speed))
            _stateMachine.Emit(speed.Velocity.IsZeroApprox() ? State.Idle : State.Walking);
    }

    void on_attack()
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

    void on_shoot()
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

    void on_charge()
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
                on_return();
                break;
        }
    }

    void on_reach_target()
    {
        if (this.TryGetComponent<AIComponent>(out var ai) && ai.Strategy is SquadStrategy ss)
            this.Configure<PMDSprite>(p => p.Direction = ss.SquadInfo.FacingDirection);
    }

    void on_move(Vector2 dir)
    {
        switch (state)
        {
            case State.Idle or State.Walking:
                {
                    if (dir.IsZeroApprox())
                    {
                        _stateMachine.Emit(State.Idle);
                    }
                    else
                    {
                        if (dir.LengthSquared() < 40)
                            dir = dir.Rotated(Mathf.Sin(Time.GetTicksMsec() / 1000) * 0.1f);
                        _stateMachine.Emit(State.Walking);
                    }
                    if (this.TryGetComponent<SpeedComponent>(out var speed))
                        speed.Velocity = dir * speed.Speed;
                    this.Configure<PMDSprite>(p => p.Direction = dir);
                    break;
                }
        }
    }
}
