using Godot;
using System;

public class Bullet : Area2D
{
	public int Damage;
	public float Speed;
	public Vector2 Direction;

	public override void _PhysicsProcess(float delta)
	{
		Position += Direction.Normalized() * Speed;

		Godot.Collections.Array playersHit = this.GetOverlappingBodies();
		foreach (Goblin g in playersHit) {
			g.TakeDamage(Damage);
			GetParent().RemoveChild(this);
		}
	}
	
	private void OnGroundDetectBodyEntered(object body)
	{
		GetParent().RemoveChild(this);
	}
}


