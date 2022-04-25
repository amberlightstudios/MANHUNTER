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
			MarginContainer n = (MarginContainer) Next.GetParent();
			MarginContainer r = (MarginContainer) Restart.GetParent();
			n.Visible = false;
			r.Visible = false;
			MainMenu.GrabFocus();
		}
		
		if (Globals.LevelSelected == 8) {
			MarginContainer n = (MarginContainer) Next.GetParent();
			n.Visible = false;
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
