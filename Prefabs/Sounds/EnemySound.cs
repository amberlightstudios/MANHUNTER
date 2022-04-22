using Godot;
using System;

public class EnemySound : AudioStreamPlayer2D
{
	private AudioStream Slash;
	private AudioStream Smash;
	private AudioStream Shoot;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Slash = (AudioStream) ResourceLoader.Load("res://Sounds/Swipe.wav");
		Smash = (AudioStream) ResourceLoader.Load("res://Sounds/Melee.wav");
		Shoot = (AudioStream) ResourceLoader.Load("res://Sounds/Shoot.wav");
	}

	public void PlaySound(string name) 
	{
		switch (name) 
		{
		case "Slash":
			Stream = Slash;
			break;
		case "Smash":
			Stream = Smash;
			break;
		case "Shoot":
//			Stream = Shoot;
			break;
		default:
//				GD.Print(name + " does not have audio associated with it!");
			break;
		}
		Play();
	}
}
