using Godot;
using System;

public class LevelSelect : Control
{

	public override void _Ready()
	{
		Button L1 = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/1/1");
		Button L2 = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/2/2");
//		Button L3 = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/3/3");

		L1.GrabFocus();
//		L1.Connect("pressed", this, nameof(PlayGame));
//		L2.Connect("pressed", this, nameof(HostGame));
//		L3.Connect("pressed", this, nameof(JoinGame));
	}

	private void _on_1_pressed()
	{
		Globals.LastPlayedLevel = ("res://Scenes/1.tscn");
		GetTree().ChangeScene("res://Scenes/1.tscn");
	}


	private void _on_2_pressed()
	{
		Globals.LastPlayedLevel = ("res://Scenes/2.tscn");
		GetTree().ChangeScene("res://Scenes/2.tscn");
	}
}



