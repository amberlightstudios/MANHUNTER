using Godot;
using System;

public class LevelSelect : Control
{
	int levelSelected = 1;
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
	
	public override void _Process(float delta)
	{
		// TODO: change logic to on enter pressed anywhere inside the main menu and update the state 
	}

	
	private void _on_1_pressed()
	{
		Globals.LastPlayedLevel = 1;
		GetTree().ChangeScene($"res://Scenes/Levels/{Globals.LastPlayedLevel}.tscn");
	}


	private void _on_2_pressed()
	{
		Globals.LastPlayedLevel = 2;
		GetTree().ChangeScene($"res://Scenes/Levels/{Globals.LastPlayedLevel}.tscn");
	}
}



