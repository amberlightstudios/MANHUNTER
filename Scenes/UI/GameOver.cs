using Godot;
using System;

public class GameOver : Control
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Button Restart = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/Restart/Restart");
		Button MainMenu = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/MainMenu/MainMenu");
		Button Quit = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/Quit/Quit");
		Restart.GrabFocus();
		Restart.Connect("pressed", this, nameof(RestartLevel));
		MainMenu.Connect("pressed", this, nameof(MainMenu));
		Quit.Connect("pressed", this, nameof(QuitGame));	
	
	}

	private void RestartLevel()
	{
		GD.Print(Globals.LastPlayedLevel);
		GetTree().ChangeScene(Globals.LastPlayedLevel);
	}


	private void MainMenu()
	{
		GetTree().ChangeScene("res://Scenes/UI/LevelSelect.tscn");
	}


	private void QuitGame()
	{
		GetTree().Quit();
	}
}



