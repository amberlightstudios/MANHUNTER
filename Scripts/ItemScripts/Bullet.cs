using Godot;
using System;
using System.Threading.Tasks;

public class Bullet : Area2D
{
	public int Damage;
	public float Speed;
	public Vector2 Direction;
	public float Range;
	private Area2D groundDetect;
	private AnimationPlayer animPlayer;

	private float startX;

	public override void _Ready() 
	{
		groundDetect = GetNode<Area2D>("GroundDetect");
		startX = Position.x;
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _PhysicsProcess(float delta)
	{
		// Bullet should only move horizontally. 
		Position += new Vector2(Math.Sign(Direction.x), 0) * Speed;

		if (Math.Abs(Position.x - startX) > Range) {
			GetParent().RemoveChild(this);
		}

		// Can hit multiple times in a single frame. (Need to fix later)
		Godot.Collections.Array playersHit = this.GetOverlappingAreas();
		foreach (Area2D g in playersHit) {
			((Goblin) g.GetParent()).TakeDamage(Damage);
			PlayBulletHit();
		}

		Godot.Collections.Array groundHit = groundDetect.GetOverlappingBodies();
		if (groundHit.Count > 0) {
			PlayBulletHit();
		}
	}

	async Task PlayBulletHit()
	{
		animPlayer.Play("Hit");
		await Task.Delay(500);
		GetParent().RemoveChild(this);
	}
}


