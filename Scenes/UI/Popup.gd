extends Control


func _ready():
	$Panel/AnimationPlayer.play("Popup")
	get_node("Panel/Restart").grab_focus()


func _on_Restart_pressed():
	$Panel/AnimationPlayer.play_backwards("Popup")
	yield(get_tree().create_timer(0.6), "timeout")
	get_tree().change_scene("res://Scenes/UI/Menu.tscn")
