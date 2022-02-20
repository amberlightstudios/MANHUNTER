extends Control

onready var restart = $"MarginContainer/VBoxContainer/Buttons/MarginContainer/Restart"

func _ready():
	restart.grab_focus()
	
func _on_Start_pressed():
	get_tree().change_scene("res://Scenes/Main.tscn")

func _on_Main_Menu_pressed():
	get_tree().change_scene("res://Scenes/UI/Menu.tscn")

func _on_Quit_pressed():
	get_tree().quit()
