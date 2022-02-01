using Godot;
using System;

public class StaticShooter : RigidBody2D
{
	private Area2D shootRange;

	public override void _Ready() 
	{
		shootRange = GetNode<Area2D>("ShootRange");
	}

	public override void _Process(float delta)
	{
		Godot.Collections.Array playersInRange = shootRange.GetOverlappingBodies();
		Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;
		foreach (Goblin p in playersInRange) {
			Godot.Collections.Dictionary result =  spaceState.IntersectRay(Position, p.Position, collisionLayer: 8);
			// No Ground layer intersected. 
			if (result.Count == 0) {
				GD.Print("Good to shoot");
			}
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		
	}
}
