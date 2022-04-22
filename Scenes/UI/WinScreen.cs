using Godot;
using System;

public class WinScreen : Control
{
	public override void _Ready()
	{
		Button Restart = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/Restart/Restart");
		Button Next = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/Next/Next");
		Button MainMenu = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/MainMenu/MainMenu");
		Next.GrabFocus();
		
		Restart.Connect("pressed", this, nameof(RestartLevel));
		MainMenu.Connect("pressed", this, nameof(MainMenu));
		Next.Connect("pressed", this, nameof(NextLevel));
		
		if (Globals.CurrentLevelMulti) {
			Next.Visible = false;
			Restart.Visible = false;
			MainMenu.GrabFocus();
		}
		
		((MenuSound) GetNode("../MenuSound")).PlaySound("Win");
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

	private void NextLevel()
	{
		((MenuSound) GetNode("../MenuSound")).Stop();
		Globals.LevelSelected++;
		GetTree().ChangeScene(Globals.GetPathToLevel(Globals.LevelSelected.ToString()));
	}
}
