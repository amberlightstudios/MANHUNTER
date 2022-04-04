using Godot;
using System;

public class GoblinSound : AudioStreamPlayer2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	private AudioStream StreamAttack;
	private AudioStream StreamJump;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		StreamAttack = (AudioStream) ResourceLoader.Load("res://Sounds/Melee.wav");
		StreamJump = (AudioStream) ResourceLoader.Load("res://Sounds/Jump.wav");
	}

	public void PlaySound(string name) 
	{
		switch (name) {
			case "BasicIdle":
			case "BasicIdle2":
			case "BasicMoving":
			case "BasicMoving2":
				Stream = StreamAttack;
				Play();
				break;
			case "Jump":
//				Stream = StreamJump;
//				Play();
//				break;
			default:
//				GD.Print(name + " does not have audio associated with it!");
				break;
		}
	}
}
