extends CharacterBody2D

enum State {
	idle = 0,
	walking = 1,
	attacking = 2,
}

var state: State = State.idle

func _physics_process(_delta: float) -> void:
	if state == 0 or state == 1:
		var vector = Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down")
		velocity = vector * 100
		move_and_slide()
		$PmdSprite.direction = velocity
		if velocity.is_zero_approx():
			state = State.idle
		else:
			state = State.walking

func _ready():
	$PmdSprite.idle_down()
	$PmdSprite.on_hit.connect(on_hit)
	$PmdSprite.on_anim_finish.connect(on_return)
	state = State.idle

func on_hit():
	print("hit")

func on_return():
	state = State.idle

func _unhandled_key_input(event: InputEvent) -> void:
	if event.is_action("attack"):
		state = State.attacking
		$PmdSprite.attack()
