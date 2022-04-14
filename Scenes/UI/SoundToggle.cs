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

 	private void _on_Button_pressed()
	{
		if (isOn) animPlayer.Play("On");
		else animPlayer.Play("Off");

		AudioServer.SetBusMute(AudioServer.GetBusIndex("BGM"), !isOn);
		isOn = !isOn;
	}
}



