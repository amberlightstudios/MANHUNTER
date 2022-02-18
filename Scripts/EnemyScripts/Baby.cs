using Godot;
using System;
using BabyStates;

public class Baby : Enemy
{
	[Export]
	public float Speed { get; private set; }
	[Export]
	public float JumpSpeed { get; private set; }
	[Export]
	public float attackRange { get; private set; }
	[Puppet]
	public Vector2 BabyPuppetPosition { get; set; }
	[Puppet]
	public Vector2 BabyPuppetVelocity { get; set; }
	
	private RayCast2D edgeDetectLeft, edgeDetectRight, wallDetect;
	public RayCast2D TopDetect;
	private GameManager gamemanager;
	public RayCast2D PlayerDetect { get; private set; }
	public RayCast2D PlayerDetectBack{ get; private set; }
	public Goblin Player;

	public BabyState State;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		edgeDetectLeft = GetNode<RayCast2D>("Sprite/EdgeDetectLeft");
		edgeDetectRight = GetNode<RayCast2D>("Sprite/EdgeDetectRight");
		TopDetect = GetNode<RayCast2D>("TopDetect");
		wallDetect = GetNode<RayCast2D>("Sprite/WallDetect");
		PlayerDetect = GetNode<RayCast2D>("Sprite/PlayerDetect");
		PlayerDetectBack = GetNode<RayCast2D>("Sprite/PlayerDetectBack");
		sprite = GetNode<Sprite>("Sprite");
		gamemanager = GetParent().GetNode<GameManager>("GameManager");
		State = new MoveState(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		// For debug drawing in _Draw
		Update();
		State._Process(delta);
	}

	public override void _PhysicsProcess(float delta)
	{
		// Networking part
		var isMultiPlayer = GetTree().NetworkPeer != null;
		if (isMultiPlayer) {
			if (GetTree().IsNetworkServer()) {
				updateBaby(delta);
				BroadcastState();
			}	
			else {
				ReceiveState();
			}
		} else {
			updateBaby(delta);
		}
	}
	
	private void updateBaby(float delta) {
		if (isTakingDamage)
			return;	

		State._PhysicsProcess(delta);

		velocity.y += Gravity;
		MoveAndSlide(velocity);
	}

	public void BroadcastState() 
	{
		Rset(nameof(BabyPuppetPosition), Position);			
		Rset(nameof(BabyPuppetVelocity), Velocity);
	}
	
	public void ReceiveState() 
	{
		Position = BabyPuppetPosition;
		Velocity = BabyPuppetVelocity;
	}
	
	public bool OnGround() 
	{
		return GetNode<RayCast2D>("GroundDetect").IsColliding();
	}

	public void CheckEdge() 
	{
		// if the enemy is actually falling down, then we do nothing. 
		if (!(edgeDetectLeft.IsColliding() || edgeDetectRight.IsColliding())) {
			return;
		}

		if ((!edgeDetectLeft.IsColliding() || wallDetect.IsColliding()) && velocity.x < 0) {
			TurnRight();
		} else if ((!edgeDetectRight.IsColliding() || wallDetect.IsColliding()) && velocity.x > 0) {
			TurnLeft();
		}
	}

	public override void TurnLeft() 
	{
		sprite.Scale = new Vector2(Math.Abs(sprite.Scale.x), sprite.Scale.y);
	}

	public override void TurnRight() 
	{
		sprite.Scale = new Vector2(-1 * Math.Abs(sprite.Scale.x), sprite.Scale.y);
	}

	public bool PlayerInAttackRange() 
	{
		return Math.Abs(Player.Position.x - Position.x) < attackRange;
	}

	public float GetAttackDist() 
	{
		return Player.Position.x - Position.x;
	}

	public override void Death() 
	{
		isTakingDamage = false;
		touchDamage = 0;
		State.ExitState(new DeathState(this));
	}

	public override void _Draw()
	{
		// DrawLine(Vector2.Zero, new Vector2(attackRange, 0), new Color(0, 0, 0, 1));
		// DrawLine(Vector2.Zero, new Vector2(-attackRange, 0), new Color(0, 0, 0, 1));
	}
}
