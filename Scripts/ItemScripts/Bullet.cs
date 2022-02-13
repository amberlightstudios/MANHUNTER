using Godot;
using System;

public class Bullet : Area2D
{
	public int Damage;
	public float Speed;
	public Vector2 Direction;
	private Area2D groundDetect;

	public override void _Ready() 
	{
		groundDetect = GetNode<Area2D>("GroundDetect");
	}

	public override void _PhysicsProcess(float delta)
	{
		// Bullet should only move horizontally. 
		Position += new Vector2(Math.Sign(Direction.x), 0) * Speed;

		Godot.Collections.Array playersHit = this.GetOverlappingAreas();
		foreach (Area2D g in playersHit) {
			((Goblin) g.GetParent()).TakeDamage(Damage);
			GetParent().RemoveChild(this);
		}

		Godot.Collections.Array groundHit = groundDetect.GetOverlappingBodies();
		if (groundHit.Count > 0) {
			GetParent().RemoveChild(this);
		}
	}

}


