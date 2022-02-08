using Godot;
using System;
using ShooterStates;

public class StaticShooter : Enemy
{
	[Export]
	private float shootFrequency = 2f;  // The enemy shoots every shootFrequency seconds
	public float ShootFrequency { get => shootFrequency; }
	[Export]
	private float bulletSpeed;
	[Export]
	private int damage = 1;
	private Sprite sprite;
	private Area2D shootRange;
	private Node2D shootPoint;
	private RayCast2D groundDetect;

	public ShooterState State;

	public override void _Ready() 
	{
		sprite = GetNode<Sprite>("Sprite");
		shootRange = GetNode<Area2D>("Sprite/ShootRange");
		shootPoint = GetNode<Node2D>("Sprite/ShootPoint");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		groundDetect = GetNode<RayCast2D>("GroundDetect");

		State = new StaticState(this, shootFrequency);
	}

	public override void _Process(float delta)
	{
		State._Process(delta);
	}

	public override void _PhysicsProcess(float delta)
	{
		State._PhysicsProcess(delta);

		velocity.y += Gravity;
		MoveAndSlide(velocity);
	}

	public bool OnGround() {
		return groundDetect.IsColliding();
	}

	public Goblin DetectPlayer() 
	{
		Godot.Collections.Array playersInRange = shootRange.GetOverlappingBodies();
		Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;
		foreach (Goblin p in playersInRange) {
			Godot.Collections.Dictionary result =  spaceState.IntersectRay(Position, p.Position, collisionLayer: 8);
			// No Ground layer intersected. 
			if (result.Count == 0) {
				return p;
			}
		}

		return null;
	}

	public void Shoot(Goblin target) 
	{
		PackedScene bulletLoader = ResourceLoader.Load<PackedScene>("res://Prefabs/Items/Bullet.tscn");
		Bullet bullet = bulletLoader.Instance<Bullet>();
		bullet.Damage = damage;
		bullet.Speed = bulletSpeed;
		bullet.Direction = target.Position - shootPoint.GlobalPosition;
		bullet.Position = shootPoint.GlobalPosition;
		GetParent().AddChild(bullet);
	}

	public void TurnLeft() 
	{
		sprite.Scale = Vector2.One;
	}

	public void TurnRight() 
	{
		sprite.Scale = new Vector2(-1, 1);
	}

	public override void TakeDamage(int dmg, Vector2 knockbackDist)
	{
		base.TakeDamage(dmg, knockbackDist * 0.4f);
		
	}

	public override void Death()
	{
		State.ExitState(new DeathState(this));
	}
}
