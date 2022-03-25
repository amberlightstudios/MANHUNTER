using Godot;
using System;

public class LevelSelect : Control
{
	public override void _Ready()
	{
		Button LevelBtn = (Button) GetNode($"MarginContainer/VBoxContainer/Buttons/{Globals.LevelSelected}/{Globals.LevelSelected}");
		LevelBtn.GrabFocus();
		FindAllLevels();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("ui_up")) {
			if (Globals.LevelSelected == 1) return;
			Globals.LevelSelected = (Globals.LevelSelected - 1) % Globals.NumLevels;
//			GD.Print("Level " + levelSelected);
		}
		else if (inputEvent.IsActionPressed("ui_down")) {
			if (Globals.LevelSelected == Globals.NumLevels) return;
			Globals.LevelSelected = (Globals.LevelSelected + 1);
//			GD.Print("Level " + levelSelected);
		}
		else if (inputEvent.IsActionPressed("ui_accept")) {
//			GD.Print("Level " + levelSelected);
			if (Globals.SinglePlayer) 
				GetTree().ChangeScene(Globals.GetPathToLevel(Globals.LevelSelected.ToString()));
			else
				GetTree().ChangeScene(Globals.PathToNetwork);
		}
	}

	private void FindAllLevels()
	{
		if (Globals.LevelsLoaded) return;
		var dir = new Directory();
		dir.Open("res://Scenes/Levels");
		dir.ListDirBegin();
		var fileName = dir.GetNext();
		while (fileName != "") {
			if (!fileName.BeginsWith(".")) {
				Globals.NumLevels++;
				GD.Print(fileName);
			}
			fileName = dir.GetNext();
		}
		Globals.LevelsLoaded = false;
	}
}
