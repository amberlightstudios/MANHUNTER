using Godot;
using System;

public class GameOver : Control
{
	public override void _Ready()
	{
		Button Restart = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/Restart/Restart");
		Button MainMenu = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/MainMenu/MainMenu");
		Button Quit = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/Quit/Quit");
		Restart.GrabFocus();
		Restart.Connect("pressed", this, nameof(RestartLevel));
		MainMenu.Connect("pressed", this, nameof(MainMenu));
		Quit.Connect("pressed", this, nameof(QuitGame));
		
		((MenuSound) GetNode("../MenuSound")).PlaySound("GameOver");
	}

	private void RestartLevel()
	{
		((MenuSound) GetNode("../MenuSound")).Stop();
		GetTree().ChangeScene(Globals.GetPathToLevel(Globals.LevelSelected.ToString()));
	}

	private void MainMenu()
	{
		GetTree().ChangeScene("res://Scenes/UI/Menu.tscn");
	}

	private void QuitGame()
	{
		GetTree().Quit();
	}
}
