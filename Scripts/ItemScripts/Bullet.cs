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
		Position += Direction.Normalized() * Speed;

		Godot.Collections.Array playersHit = this.GetOverlappingBodies();
		foreach (Goblin g in playersHit) {
			g.TakeDamage(Damage);
			GetParent().RemoveChild(this);
		}

		Godot.Collections.Array groundHit = groundDetect.GetOverlappingBodies();
		if (groundHit.Count > 0) {
			GetParent().RemoveChild(this);
		}
	}

}


