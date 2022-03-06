using Godot;
using System;
using System.Threading.Tasks;

public class LevelSelect : Control
{
	int levelSelected = 1;
	
	private AnimationPlayer toggle;
	private AnimationPlayer fader;
	
	private Button levelSelector;

	public override void _Ready()
	{
		toggle = (AnimationPlayer) GetNode("MarginContainer/VBoxContainer/Buttons/Level/AnimationPlayer");
		fader = (AnimationPlayer) GetNode("Fader/AnimationPlayer");
		levelSelector = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/Level/Number");
		levelSelector.Text = "LEVEL " + levelSelected;
		
		FindAllLevels();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("ui_left")) {
			toggle.Play("GrowLeft");
			if (levelSelected == 1) levelSelected = Globals.NumLevels;
			else levelSelected --;
			levelSelector.Text = "LEVEL " + levelSelected;
		}
		else if (inputEvent.IsActionPressed("ui_right")) {
			toggle.Play("GrowRight");
			if (levelSelected == Globals.NumLevels) levelSelected = 1;
			else levelSelected++;
			levelSelector.Text = "LEVEL " + levelSelected;
		}
		else if (inputEvent.IsActionPressed("ui_accept")) {
			Globals.LastPlayedLevel = levelSelected.ToString();
			FadeIntoLevel();
		}
	}
	
	async Task FadeIntoLevel()
	{
		fader.Play("Fade");
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
			}
			fileName = dir.GetNext();
		}
		Globals.LevelsLoaded = true;
	}
}






