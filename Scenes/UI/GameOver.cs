using Godot;
using System;

public class GameOver : Control
{
	public override void _Ready()
	{
		Button Restart = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/Restart/Restart");
		Button MainMenu = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/MainMenu/MainMenu");
//		Button Quit = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/Quit/Quit");
		Restart.GrabFocus();
		Restart.Connect("pressed", this, nameof(RestartLevel));
		MainMenu.Connect("pressed", this, nameof(MainMenu));
//		Quit.Connect("pressed", this, nameof(QuitGame));
		
		((MenuSound) GetNode("../MenuSound")).PlaySound("GameOver");
		
		Label msg = (Label) GetNode("MarginContainer/VBoxContainer/MarginContainer/Label");
		if (!Globals.CurrentLevelMulti) {
			msg.Text = "u died bro. try again?";
			Restart.Visible = false;
		}
		else  msg.Text = "u all died bro. better luck next time?";
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
