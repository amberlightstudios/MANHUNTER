extends CanvasLayer

var selected = 0
var noopen = ["res://Scenes/UI/Menu.tscn", "res://Scenes/UI/GameOver.tscn"]

func _ready():
	set_visible(false)


func _input(event):
	if event.is_action_pressed("Pause"):
		if get_tree().current_scene.filename in noopen:
			return
		set_visible(!get_tree().paused)
		get_tree().paused = !get_tree().paused


func _on_Continue_pressed():
	get_tree().paused = false
	set_visible(false)


func set_visible(is_visible):
	for node in get_children():
		node.visible = is_visible


func _on_Quit_pressed():
	get_tree().quit()
