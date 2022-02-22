extends Control

onready var level1 = $"MarginContainer/VBoxContainer/Buttons/1/1"

func _ready():
	level1.grab_focus()

func _on_1_pressed():
	get_tree().change_scene("res://Scenes/TutorialLevel.tscn")

func _on_2_pressed():
	get_tree().change_scene("res://Scenes/2.tscn")
