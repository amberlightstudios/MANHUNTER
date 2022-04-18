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
	private AudioStream Checkpoint;
	private AudioStream GameOver;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Select = (AudioStream) ResourceLoader.Load("res://Sounds/UI/Select.wav");
		Confirm = (AudioStream) ResourceLoader.Load("res://Sounds/UI/Confirm.wav");
		Win = (AudioStream) ResourceLoader.Load("res://Sounds/UI/LevelComplete.wav");
		Checkpoint = (AudioStream) ResourceLoader.Load("res://Sounds/UI/Checkpoint.wav");
		GameOver = (AudioStream) ResourceLoader.Load("res://Sounds/UI/GameOver.wav");
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
			Stream = Checkpoint;
			break;
		case "Win":
			Stream = Win;
			break;
		case "GameOver":
			Stream = GameOver;
			break;
		default:
//				GD.Print(name + " does not have audio associated with it!");
			break;
		}
		Play();
	}
}
