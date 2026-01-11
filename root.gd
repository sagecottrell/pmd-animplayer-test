extends Node2D

@export var spawn_points: Array[Node2D]
@export var spawn_timer: float = 10
@export var spawn_scene: PackedScene

var last_spawned = null

var timer: float = 0

func _ready() -> void:
	timer = spawn_timer
	last_spawned = $player

func _process(delta: float) -> void:
	timer += delta
	
	if timer >= spawn_timer:
		timer -= spawn_timer
		var pt = spawn_points.pick_random()
		var new: Node2D = spawn_scene.instantiate()
		add_child(new)
		new.global_position = pt.global_position
		new.target = last_spawned
		last_spawned = new
