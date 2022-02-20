extends Control

onready var start_btn = $MarginContainer/VBoxContainer/Buttons/MarginContainer/Start

func _ready():
	start_btn.grab_focus()


func _on_Play_pressed():
	get_tree().change_scene("res://Scenes/Main.tscn")


func _on_Host_pressed():
	get_tree().change_scene("res://Scenes/Main.tscn")


func _on_Join_pressed():
	pass # Replace with function body.


func _on_Quit_pressed():
	get_tree().quit()
