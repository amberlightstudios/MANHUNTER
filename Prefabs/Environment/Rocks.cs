using Godot;
using System;

public class Rocks : KinematicBody2D
{
	// Declare member variables here. Examples:
	private AnimationPlayer animPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		animPlayer.Play("shine");
	}
}
