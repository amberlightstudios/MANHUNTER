using Godot;
using System;

public class Bomb : RigidBody2D
{
	[Export]
	private float explodeTimer = 2f;
	
	[Export]
	private int damage;

	private Area2D explodeArea;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		explodeArea = GetNode<Area2D>("ExplodeArea");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		explodeTimer -= delta;
		if (explodeTimer <= 0) {
			Explode();
		}
	}

	public void Explode() 
	{
		Godot.Collections.Array overlapObjs = explodeArea.GetOverlappingBodies();
		foreach (Character p in overlapObjs) {
			p.TakeDamage(damage);
		}
		// Destroy bomb
		GetParent().RemoveChild(this);
	}
}
