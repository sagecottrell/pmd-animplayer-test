@tool
extends CharacterBody2D

enum State {
	idle = 0,
	walking = 1,
	attacking = 2,
}

var state: State = State.idle

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

@export var target: Node2D:
	set(value):
		target = value
		if is_node_ready():
			$AIComponent.Target = value

func _physics_process(_delta: float) -> void:
	match state:
		State.walking:
			if move_and_slide():
				$PmdSprite.Idle()
				state = State.idle
			else:
				$PmdSprite.Walk()
	

func _ready():
	state = State.idle
	$PmdSprite.Sprites = sprites
	$TeamComponent.TeamId = team
	$AIComponent.Target = target
	$PmdSprite.OnHit.connect(on_hit)
	$PmdSprite.OnAnimFinish.connect(on_return)
	
	$HurtComponent.OnHurt.connect(on_hurt)
	
	$HealthComponent.OnDeath.connect(on_death)
	
	$AIComponent.OnNewVelocity.connect(on_move)
	
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
		state = State.idle 
		$PmdSprite.Idle()
	else:
		state = State.walking
		$PmdSprite.Walk()

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
				$PmdSprite.Idle()
			else:
				state = State.walking
				$PmdSprite.Walk()
