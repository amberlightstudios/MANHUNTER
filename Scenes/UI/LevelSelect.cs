using Godot;
using System;
using System.Threading.Tasks;

public class LevelSelect : Control
{
	int levelSelected = 1;
	
	private AnimationPlayer animPlayer;

	public override void _Ready()
	{
		animPlayer = (AnimationPlayer) GetNode("Fader/AnimationPlayer");
		Button L1 = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/1/1");
		L1.GrabFocus();
		
		FindAllLevels();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("ui_up")) {
			if (levelSelected == 1) return;
			levelSelected = (levelSelected - 1) % Globals.NumLevels;
//			GD.Print("Level " + levelSelected);
		}
		else if (inputEvent.IsActionPressed("ui_down")) {
			if (levelSelected == Globals.NumLevels) return;
			levelSelected = (levelSelected + 1);
//			GD.Print("Level " + levelSelected);
		}
		else if (inputEvent.IsActionPressed("ui_accept")) {
//			GD.Print("Level " + levelSelected);
			Globals.LastPlayedLevel = levelSelected.ToString();
			FadeIntoLevel();
		}
	}
	
	async Task FadeIntoLevel()
	{
		animPlayer.Play("Fade");
		await Task.Delay(700);
		if (Globals.SinglePlayer) 
			GetTree().ChangeScene(Globals.GetPathToLevel(levelSelected.ToString()));
		else
			GetTree().ChangeScene(Globals.PathToNetwork);
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






