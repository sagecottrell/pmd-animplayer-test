@tool
extends CharacterBody2D

@export var sprites: AnimationLibrary:
	get:
		return sprites
	set(value):
		sprites = value
		if is_node_ready():
			$PmdSprite.Sprites = value

@export var team: int:
	set(value):
		team = value
		if is_node_ready():
			$TeamComponent.TeamId = value
	
enum State {
	idle = 0,
	walking = 1,
	attacking = 2,
}

var state: State = State.idle
var state_machine: StateMachine = StateMachine.Create({
	State.idle: on_idle,
	State.walking: on_walking,
	State.attacking: on_attacking,
})

func on_idle():
	velocity = Vector2.ZERO
	state = State.idle
	$PmdSprite.Idle()

func on_walking():
	state = State.walking
	$PmdSprite.Walk()

func on_attacking():
	velocity = Vector2.ZERO
	state = State.attacking

var input_buffer = noop
func noop():
	pass
	
func _physics_process(_delta: float) -> void:
	match state:
		State.walking:
			if move_and_slide():
				state_machine.Emit(State.idle)

func _ready():
	$PmdSprite.Sprites = sprites
	$TeamComponent.TeamId = team
	$PmdSprite.OnHit.connect(on_hit)
	$PmdSprite.OnAnimFinish.connect(on_return)
	
	$UserInputComponent.OnMovement.connect(on_move)
	$UserInputComponent.OnAttack.connect(on_attack)
	$UserInputComponent.OnShoot.connect(on_shoot)
	$UserInputComponent.OnCharge.connect(on_charge)
	
	$HurtComponent.OnHurt.connect(on_hurt)
	
	$HealthComponent.OnDeath.connect(on_death)
	
	on_return()

func on_hit():
	print("hit")

func on_death():
	pass

func on_hurt(hurt):
	$HealthComponent.TakeDamage(hurt)
	$PmdSprite.Hurt()

func on_return():
	if velocity.is_zero_approx():
		state_machine.Emit(State.idle)
	else:
		state_machine.Emit(State.walking)
	
	input_buffer.call()
	input_buffer = noop

func on_attack() -> void:
	match state:
		State.idle, State.walking:
			state_machine.Emit(State.attacking)
			$PmdSprite.Attack()

func on_shoot():
	match state:
		State.idle, State.walking:
			state_machine.Emit(State.attacking)
			$PmdSprite.Shoot()
	
func on_charge():
	match state:
		State.idle, State.walking:
			state_machine.Emit(State.attacking)
			$PmdSprite.Charge()
		State.attacking:
			on_return()

func on_move(dir: Vector2):
	match state:
		State.idle, State.walking:
			velocity = dir * 100
			if dir.is_zero_approx():
				state_machine.Emit(State.idle)
			else:
				$PmdSprite.Direction = velocity
				state_machine.Emit(State.walking)
		_:
			input_buffer = func(): on_move(dir)
