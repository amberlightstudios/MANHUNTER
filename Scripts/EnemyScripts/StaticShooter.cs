using Godot;
using System;

public class StaticShooter : Enemy
{
	[Export]
	private float shootFrequency = 2f;  // The enemy shoots every shootFrequency seconds
	[Export]
	private float bulletSpeed;
	[Export]
	private int damage = 1;
	private float timer;
	private Sprite sprite;
	private Area2D shootRange;
	private Node2D shootPoint;
	private AnimationPlayer animPlayer;

	public override void _Ready() 
	{
		sprite = GetNode<Sprite>("Sprite");
		shootRange = GetNode<Area2D>("ShootRange");
		shootPoint = GetNode<Node2D>("Sprite/ShootPoint");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		timer = shootFrequency;
	}

	public override void _Process(float delta)
	{
		timer += delta;

		if (timer > shootFrequency) {
			Godot.Collections.Array playersInRange = shootRange.GetOverlappingBodies();
			Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;
			foreach (Goblin p in playersInRange) {
				Godot.Collections.Dictionary result =  spaceState.IntersectRay(Position, p.Position, collisionLayer: 8);
				// No Ground layer intersected. 
				if (result.Count == 0) {
					PackedScene bulletLoader = ResourceLoader.Load<PackedScene>("res://Prefabs/Items/Bullet.tscn");
					Bullet bullet = bulletLoader.Instance<Bullet>();
					bullet.Damage = damage;
					bullet.Speed = bulletSpeed;
					bullet.Direction = p.Position - shootPoint.GlobalPosition;
					bullet.Position = shootPoint.GlobalPosition;
					GetParent().AddChild(bullet);
					timer = 0;
					sprite.Scale = new Vector2(Math.Sign(bullet.Direction.x) * -1, 1);
					animPlayer.Play("Shoot");
					break;
				}
			} 
		}

		if (!animPlayer.IsPlaying()) {
			animPlayer.Play("Idle");
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);
	}

	public override void TakeDamage(int dmg, Vector2 knockbackDist)
	{
		base.TakeDamage(dmg, knockbackDist * 0.4f);
	}
}
