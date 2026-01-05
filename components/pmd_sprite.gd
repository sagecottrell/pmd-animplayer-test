@tool
extends Node2D

const THRESHOLD: float = sin(PI / 8)
var facing: String
var previous_animation: String = ""

signal on_hit()
signal on_rush()
signal on_return()
signal on_anim_finish()

@export var direction: Vector2:
	set(value):
		direction = value.normalized()
		update_anim()

@export var sprites: AnimationLibrary:
	set(value):
		sprites = value
		if %AnimationPlayer.has_animation_library(""):
			%AnimationPlayer.remove_animation_library("")
		%AnimationPlayer.add_animation_library("", value)

func _ready():
	%AnimationPlayer.animation_finished.connect(_reset_to_previous)

func idle_down():
	direction = Vector2.DOWN
	update_anim()

func _reset_to_previous(_anim):
	if previous_animation != "":
		%AnimationPlayer.play(previous_animation)
	on_anim_finish.emit()

func attack():
	previous_animation = %AnimationPlayer.current_animation
	%AnimationPlayer.play("Attack-%s" % facing)

func update_anim():
	if direction.is_zero_approx():
		%AnimationPlayer.play("Idle-%s" % facing)
		return
	facing = ""
	if direction.y < -THRESHOLD:
		facing += "up"
	elif direction.y > THRESHOLD:
		facing += "down"
	if direction.x < -THRESHOLD:
		facing += "left"
	elif direction.x > THRESHOLD:
		facing += "right"
	%AnimationPlayer.play("Walk-%s" % facing)

func on_hit_frame():
	on_hit.emit()

func on_rush_frame():
	on_rush.emit()

func on_return_frame():
	on_return.emit()
