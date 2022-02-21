extends CanvasLayer

var selected = 0
var noopen = [
	"res://Scenes/UI/Menu.tscn", 
	"res://Scenes/UI/GameOver.tscn", 
]
	
var goback = [
	"res://Scenes/UI/JoinGame.tscn",
]

func _ready():
	set_visible(false)


func _input(event):
	if event.is_action_pressed("Pause"):
		if get_tree().current_scene.filename in noopen:
			return
		elif get_tree().current_scene.filename in goback:
			get_tree().change_scene("res://Scenes/UI/Menu.tscn")
			return
		set_visible(!get_tree().paused)
		get_tree().paused = !get_tree().paused
		
	if event.is_action_pressed("Quit"):
		if get_tree().paused == false:
			return
		get_tree().change_scene("res://Scenes/UI/Menu.tscn")
		_on_Continue_pressed()


func _on_Continue_pressed():
	get_tree().paused = false
	set_visible(false)


func set_visible(is_visible):
	for node in get_children():
		node.visible = is_visible


func _on_Quit_pressed():
	get_tree().quit()
