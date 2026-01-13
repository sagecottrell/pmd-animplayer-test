@tool
extends CharacterBody2D

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
	$PmdSprite.Attack()

@export var clickable: bool:
	set(value):
		clickable = value
		if is_node_ready():
			$UnitClickArea.SetEnabled(value)

@export var sprites: AnimationLibrary:
	get:
		return sprites
	set(value):
		sprites = value
		if is_node_ready():
			$PmdSprite.Sprites = value

@export var team: TeamInfo:
	set(value):
		team = value
		if is_node_ready():
			$TeamComponent.Team = value

@export var target: Node2D:
	set(value):
		target = value
		if is_node_ready():
			$AIComponent.Target = value

func _physics_process(_delta: float) -> void:
	match state:
		State.walking, State.idle:
			if velocity.is_zero_approx() or move_and_slide():
				state_machine.Emit(State.idle)
			else:
				state_machine.Emit(State.walking)
	

func _ready():
	state = State.idle
	$PmdSprite.Sprites = sprites
	$TeamComponent.Team = team
	$AIComponent.Target = target
	$PmdSprite.OnHit.connect(on_hit)
	$PmdSprite.OnAnimFinish.connect(on_return)
	
	$HurtComponent.OnHurt.connect(on_hurt)
	
	$HealthComponent.OnDeath.connect(on_death)
	
	$AIComponent.OnNewVelocity.connect(on_move)
	$UnitClickArea.input_pickable = clickable
	
	var collide_shape = $UnitClickArea.GetShape()
	if collide_shape is CircleShape2D:
		$SelectableComponent.SelectionRadius = collide_shape.radius \
		 - $SelectableComponent.SelectionWidth / 2
	
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
			$PmdSprite.Direction = dir
			if dir.is_zero_approx() or get_last_slide_collision() != null:
				state_machine.Emit(State.idle)
			else:
				state_machine.Emit(State.walking)
