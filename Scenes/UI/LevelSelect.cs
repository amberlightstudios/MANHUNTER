using Godot;
using System;
using System.Threading.Tasks;

public class LevelSelect : Control
{
	private bool levelSelected;

	private AnimationPlayer toggle;
	private AnimationPlayer fader;
	
	private Button levelSelector;

	public override void _Ready()
	{
		levelSelected = false;

		toggle = (AnimationPlayer) GetNode("MarginContainer/VBoxContainer/Buttons/Level/AnimationPlayer");
		fader = (AnimationPlayer) GetNode("Fader/AnimationPlayer");
		levelSelector = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/Level/Number");
		levelSelector.Text = "LEVEL " + (Globals.LevelSelected - 1);

		FindAllLevels();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (levelSelected) return;
		if (inputEvent.IsActionPressed("ui_left")) {
			((MenuSound) GetNode("../MenuSound")).PlaySound("Select");
			toggle.Play("GrowLeft");
			if (Globals.LevelSelected == 1) Globals.LevelSelected = Globals.NumLevels;
			else Globals.LevelSelected--;
			levelSelector.Text = "LEVEL " + (Globals.LevelSelected - 1);
		}
		else if (inputEvent.IsActionPressed("ui_right")) {
			((MenuSound) GetNode("../MenuSound")).PlaySound("Select");
			toggle.Play("GrowRight");
			if (Globals.LevelSelected == Globals.NumLevels) Globals.LevelSelected = 1;
			else Globals.LevelSelected++;
			levelSelector.Text = "LEVEL " + (Globals.LevelSelected - 1);
		}
		else if (inputEvent.IsActionPressed("ui_accept")) {
			FadeIntoLevel();
		}
	}
	
	async Task FadeIntoLevel()
	{
		levelSelected = true;
		((MenuSound) GetNode("../MenuSound")).PlaySound("Confirm");
		fader.Play("Fade");
		await Task.Delay(700);
		if (Globals.SinglePlayer) 
			GetTree().ChangeScene(Globals.GetPathToLevel(Globals.LevelSelected.ToString()));
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
