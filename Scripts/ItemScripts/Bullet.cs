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
	private bool hasHit = false;

	private bool deflecting = false;
	private float deflectTimer = 0f;

	public override void _Ready() 
	{
		groundDetect = GetNode<Area2D>("GroundDetect");
		startX = Position.x;
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _PhysicsProcess(float delta)
	{
		if (hasHit)
			return;

		if (deflecting) {
			deflectTimer += delta;
			if (deflectTimer > 0.4f) {
				deflecting = false;
				deflectTimer = 0f;
			}
		}

		// Bullet should only move horizontally. 
		Position += new Vector2(Math.Sign(Direction.x), 0) * Speed;

		if (Math.Abs(Position.x - startX) > Range) {
			GetParent().RemoveChild(this);
		}

		// Can hit multiple times in a single frame. (Need to fix later)
		Godot.Collections.Array playersHit = this.GetOverlappingAreas();
		foreach (Area2D g in playersHit) {
			if ((g.CollisionLayer & 2) == 0)
				continue;
			((Goblin) g.GetParent()).TakeDamage(Damage);
			hasHit = true;
			PlayBulletHit();
			return;
		}

		Godot.Collections.Array groundHit = groundDetect.GetOverlappingBodies();
		if (groundHit.Count > 0) {
			hasHit = true;
			PlayBulletHit();
		}

		// This part of code is only for hitting enemies.
		foreach (Character c in this.GetOverlappingBodies()) {
			if ((c.CollisionLayer & 4) == 0)
				continue;

			c.TakeDamage(Damage);
			hasHit = true;
			PlayBulletHit();
			return;
		}
	}

	public async Task PlayBulletHit()
	{
		animPlayer.Play("Hit");
		await Task.Delay(500);
		GetParent().RemoveChild(this);
	}

	// After deflection, the bullet is now also able to hit enemies
	public void Deflect() 
	{
		if (!deflecting) {
			Direction.x *= -1;
			SetCollisionMaskBit(2, true);
			deflecting = true;
		}
	}
}


