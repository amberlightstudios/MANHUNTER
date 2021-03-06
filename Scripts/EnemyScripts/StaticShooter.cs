using Godot;
using System;
using ShooterStates;

public class StaticShooter : Enemy
{
	[Export]
	private float noticeTime = 1f;
	public float NoticeTime { get => noticeTime; }
	[Export]
	private float shootFrequency = 2f;  // The enemy shoots every shootFrequency seconds
	public float ShootFrequency { get => shootFrequency; }
	[Export]
	private float bulletSpeed;
	[Export]
	private float bulletRange = 300f;
	[Export]
	private int damage = 1;
	[Export]
	private float roamSpeed = 30f;
	public float RoamSpeed { get => roamSpeed; }
	[Export]
	private float evadeDist = 90f;
	public float EvadeDist { get => evadeDist; }
	private float evadeSpeed = 200f;
	public float EvadeSpeed { get => evadeSpeed; }

	private GameManager gm;
	private Area2D shootRange;
	private Node2D shootPoint;
	private RayCast2D groundDetect, edgeDetectLeft, edgeDetectRight, wallDetect;

	public ShooterState State;

	public override void _Ready() 
	{
		sprite = GetNode<Sprite>("Sprite");
		shootRange = GetNode<Area2D>("Sprite/ShootRange");
		shootPoint = GetNode<Node2D>("Sprite/ShootPoint");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		groundDetect = GetNode<RayCast2D>("Detections/GroundDetect");
		edgeDetectLeft = GetNode<RayCast2D>("Detections/EdgeDetectLeft");
		edgeDetectRight = GetNode<RayCast2D>("Detections/EdgeDetectRight");
		wallDetect = GetNode<RayCast2D>("Sprite/WallDetect");
		gm =  GetParent().GetParent().GetNode<GameManager>("GameManager");

		State = new NormalState(this);
	}

	public override void _Process(float delta)
	{
		if (GetTree().NetworkPeer == null || GetTree().IsNetworkServer()) {
			State._Process(delta);		
		} 
		SynchronizeState();		
	}

	public override void _PhysicsProcess(float delta)
	{
		if (GetTree().NetworkPeer == null || GetTree().IsNetworkServer()) {
			State._PhysicsProcess(delta);
			if (!OnGround()) velocity.y += Gravity;
			MoveAndSlide(velocity);
		}  
		SynchronizeState();
	}
	
	private void Interpolate() {
		// TODO
	}


	public bool OnGround() {
		bool onGround = groundDetect.IsColliding();
		return onGround;
	}

	public Goblin DetectPlayer() 
	{
		Godot.Collections.Array playersInRange = shootRange.GetOverlappingAreas();
		Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;
		foreach (Area2D p in playersInRange) {
			Godot.Collections.Dictionary result =  spaceState.IntersectRay(Position, p.GlobalPosition, collisionLayer: 8);
			// No Ground layer intersected. 
			if (result.Count == 0) {
				return (Goblin) p.GetParent();
			}
		}

		return null;
	}

	public override void Shoot() 
	{
		PackedScene bulletLoader = ResourceLoader.Load<PackedScene>("res://Prefabs/Items/Bullet.tscn");
		Bullet bullet = bulletLoader.Instance<Bullet>();
		bullet.Damage = damage;
		bullet.Speed = bulletSpeed;
		bullet.Direction = sprite.Scale;
		bullet.Position = shootPoint.GlobalPosition;
		bullet.Range = bulletRange;
		GetParent().AddChild(bullet);
	}

	public void EdgeDetect() 
	{
		if ((!edgeDetectLeft.IsColliding() || wallDetect.IsColliding()) && velocity.x < 0) {
			TurnRight();
			velocity.x *= -1;
		} else if ((!edgeDetectRight.IsColliding() || wallDetect.IsColliding()) && velocity.x > 0) {
			TurnLeft();
			velocity.x *= -1;
		}
	}

	public override void TakeDamage(int dmg, Vector2 knockbackDist)
	{
		if (!Globals.SinglePlayer)
			Rpc(nameof(TakeDamageRemote), dmg, knockbackDist);
		base.TakeDamage(dmg, knockbackDist * 0.4f);
	}
	
	[Remote]
	public void TakeDamageRemote(int dmg, Vector2 knockbackDist)
	{
		base.TakeDamage(dmg, knockbackDist * 0.4f);
	}

	public override void Death()
	{
		touchDamage = 0;
		State.ExitState(new DeathState(this));
	}

	public Vector2 PlayerOffset() 
	{
		Vector2 dist = Vector2.Inf;

		for (int i = 0; i < 4; i++) {
			if (gm.PlayerList[i] != null) {
				Goblin player = gm.PlayerList[i];
				Vector2 newDist = Position - player.Position;
				dist = newDist.Length() < dist.Length() ? newDist : dist;
			}
		}
		
		return dist;
	}

	public override void _Draw()
	{
		// DrawCircle(Vector2.Zero, evadeDist/3, new Color(1, 1, 1, 1));
	}
}
