using Godot;
using System;

public class MenuSound : AudioStreamPlayer2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	private AudioStream Select;
	private AudioStream Confirm;
	private AudioStream Win;
	private AudioStream Cp;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Select = (AudioStream) ResourceLoader.Load("res://Sounds/UI/Select.wav");
		Confirm = (AudioStream) ResourceLoader.Load("res://Sounds/UI/Confirm.wav");
		Win = (AudioStream) ResourceLoader.Load("res://Sounds/UI/LevelComplete.wav");
		Cp = (AudioStream) ResourceLoader.Load("res://Sounds/UI/Checkpoint.wav");
	}
	
	public void PlaySound(string name) 
	{
		switch (name)
		{
		case "Select":
			Stream = Select;
			break;
		case "Confirm":
			Stream = Confirm;
			break;
		case "Checkpoint":
			Stream = Cp;
			break;
		case "Win":
			Stream = Win;
			break;
		default:
//				GD.Print(name + " does not have audio associated with it!");
			break;
		}
		Play();
	}
}
