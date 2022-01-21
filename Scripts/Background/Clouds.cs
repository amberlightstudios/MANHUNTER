using Godot;
using System;

public class Clouds : Sprite
{
	[Export]
	private float speed = 0.1f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	public override void _PhysicsProcess(float delta) 
	{
		Position += new Vector2(speed, 0);
	}
}
