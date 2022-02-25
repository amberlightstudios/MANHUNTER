extends Node2D

var scene = preload("res://Prefabs/Effects/Blood.tscn")
# Called when the node enters the scene tree for the first time.
func _process(delta):
	if Input.is_action_just_pressed('Jump'):
		for i in range(15):
			add_child(scene.instance())
	

