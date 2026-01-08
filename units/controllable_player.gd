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
		$TeamComponent.TeamId = value
	
enum State {
	idle = 0,
	walking = 1,
	attacking = 2,
}

var state: State = State.idle

var input_buffer = noop
func noop():
	pass
	
func _physics_process(_delta: float) -> void:
	if state == State.walking:
		move_and_slide()

func _ready():
	$PmdSprite.Sprites = sprites
	$PmdSprite.Idle()
	$PmdSprite.OnHit.connect(on_hit)
	$PmdSprite.OnAnimFinish.connect(on_return)
	
	$UserInputComponent.OnMovement.connect(on_move)
	$UserInputComponent.OnAttack.connect(on_attack)
	$UserInputComponent.OnShoot.connect(on_shoot)
	$UserInputComponent.OnCharge.connect(on_charge)
	
	$HurtComponent.OnHurt.connect($HealthComponent.TakeDamage)
	
	state = State.idle

func on_hit():
	print("hit")

func on_return():
	if velocity.is_zero_approx():
		state = State.idle 
		$PmdSprite.Idle()
	else:
		state = State.walking
		$PmdSprite.Walk()
	
	input_buffer.call()
	input_buffer = noop

func on_attack() -> void:
	match state:
		State.idle, State.walking:
			state = State.attacking
			$PmdSprite.Attack()

func on_shoot():
	match state:
		State.idle, State.walking:
			state = State.attacking
			$PmdSprite.Shoot()
	
func on_charge():
	match state:
		State.idle, State.walking:
			state = State.attacking
			$PmdSprite.Charge()
		State.attacking:
			on_return()

func on_move(dir: Vector2):
	match state:
		State.idle, State.walking:
			velocity = dir * 100
			$PmdSprite.Direction = velocity
			if velocity.is_zero_approx():
				state = State.idle
			else:
				state = State.walking
		_:
			input_buffer = func(): on_move(dir)
