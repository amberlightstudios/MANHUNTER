using Godot;
using System;

public class SoundToggle : CanvasLayer
{
	private bool BGMMuted = false;
	private AnimationPlayer animPlayer;
	private bool isOn = true;
	
	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}
	
	public override void _Process(float delta)
	{
		if (isOn) animPlayer.Play("On");
		else animPlayer.Play("Off");
		
		if (GetTree().Paused == true && isOn) {
			AudioServer.SetBusMute(AudioServer.GetBusIndex("BGM"), isOn);
		}
	}
	
	public override void _Input(InputEvent input)
	{
		if (input.IsActionPressed("Mute")) {
			if (isOn) isOn = false;
			else isOn = true;
			AudioServer.SetBusMute(AudioServer.GetBusIndex("BGM"), isOn);
		}
	}

 	private void _on_Button_pressed()
	{
		if (isOn) isOn = false;
		else isOn = true;
		AudioServer.SetBusMute(AudioServer.GetBusIndex("BGM"), isOn);
	}
}



