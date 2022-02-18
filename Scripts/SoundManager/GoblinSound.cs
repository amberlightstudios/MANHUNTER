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
		if (name == "BasicIdle" || name == "BasicIdle2" || name == "BasicMoving" || name == "BasicMoving2") {
			Stream = StreamAttack;
			Play();
		} else if (name == "Jump") {
			Stream = StreamJump;
			Play();
		}
	}
}
