using Godot;
using System;
using GoblinStates;

public class Goblin : Character
{
	public GoblinState State;

	[Export]
	public float Speed { get; private set; }

	public Vector2 Velocity;
	
	[Puppet]
	public Vector2 PuppetPosition { get; set; }
	[Puppet]
	public Vector2 PuppetVelocity { get; set; }
	[Puppet]
	public int PuppetFaceDirection { get; set; }
	[Puppet]
	public String PuppetAnimation { get; set; }

	[Export]
	public float JumpSpeed { get; private set; }

	[Export]
	public float Gravity { get; private set; }

	// This is for throwing bombs. 
	[Export]
	private Vector2 throwDirection;
	[Export]
	private float maxThrowForce = 3000f;
	// This is for throwing enemies. (Temporary disable)
	[Export]
	private Vector2 throwVelocity;
	[Export]
	private float throwDownSpeed;

	// faceDirection == -1 -> Player is facing left.. 
	// faceDirection == 1 -> Player is facing right. 
	public int FaceDirection { get; private set; }

	private AnimationPlayer animPlayer;
	public AnimationPlayer AnimPlayer { get => animPlayer; }
	private Sprite sprite;
	public Sprite PlayerSprite { get => sprite; }
	private CollisionShape2D walkCollisionBox;
	public CollisionShape2D WalkCollisionBox { get => walkCollisionBox; }

	private RayCast2D groundDetectLeft;
	private RayCast2D groundDetectRight;
	private RayCast2D throwDetect;
	public Vector2 ThrowPoint { 
		get => (GetNode<Node2D>("Sprite/ThrowPoint").Position) * sprite.Scale + sprite.Position + Position;
		private set => ThrowPoint = value; }
	public Vector2 ThrowPointScale {
		get => GetNode<Node2D>("Sprite/ThrowPoint").Scale * sprite.Scale * Scale;
		private set => ThrowPointScale = value;
	}

	private Vector2 defaultSpriteScale;

	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		walkCollisionBox = GetNode<CollisionShape2D>("WalkCollsionBox");
		sprite = GetNode<Sprite>("Sprite");
		groundDetectLeft = GetNode<RayCast2D>("GroundDetectLeft");
		groundDetectRight = GetNode<RayCast2D>("GroundDetectRight");
		throwDetect = GetNode<RayCast2D>("Sprite/ThrowDetect");
		defaultSpriteScale = sprite.Scale;

		State = new MoveState(this);
	}

	public override void _Process(float delta)
	{
		var isMultiPlayer = GetTree().NetworkPeer != null;
		if (isMultiPlayer) {
			if (IsNetworkMaster()) {
				State._Process(delta);
				BroadcastState();
			}	
			else {
				ReceiveState();
			}
		} else {
			State._Process(delta);
		}
		

		if (isMultiPlayer && !IsNetworkMaster())
			PuppetPosition = Position;
	}

	public override void _PhysicsProcess(float delta)
	{
		var isMultiPlayer = GetTree().NetworkPeer != null;
		if (isMultiPlayer) {
			if (IsNetworkMaster()) {
				State._PhysicsProcess(delta);
				BroadcastState();
				// Gravity
				Velocity.y += Gravity;
				Velocity = MoveAndSlide(Velocity);
			}	
			else {
				ReceiveState();
				PuppetPosition = Position;
			}
		} else {
			State._PhysicsProcess(delta);

			// Gravity
			Velocity.y += Gravity;
			Velocity = MoveAndSlide(Velocity);
		}
		

		// if (isMultiPlayer && !IsNetworkMaster())
		// 	PuppetPosition = Position;
	}
	
	public void BroadcastState() {
		Rset(nameof(PuppetPosition), Position);			
		Rset(nameof(PuppetVelocity), Velocity);
		Rset(nameof(PuppetFaceDirection), FaceDirection);
		Rset(nameof(PuppetAnimation), animPlayer.CurrentAnimation);	
	}
	
	public void ReceiveState() {
		Position = PuppetPosition;
		Velocity = PuppetVelocity;
		if (FaceDirection == -1 && PuppetFaceDirection == 1) {
			TurnRight();
		} else if (FaceDirection != PuppetFaceDirection) {
			TurnLeft();
		}  
		if (PuppetAnimation != null) {
			animPlayer.Play(PuppetAnimation);
		} else {
			animPlayer.Play("Idle");
		}
	}

	public void TurnLeft() 
	{
		sprite.Position = Vector2.Zero;
		sprite.Scale = defaultSpriteScale;
		FaceDirection = -1;
		throwVelocity.x = Math.Abs(throwVelocity.x) * -1;
	}

	public void TurnRight() 
	{
		sprite.Position = new Vector2(-6, 0);
		sprite.Scale = new Vector2(-defaultSpriteScale.x, defaultSpriteScale.y);
		FaceDirection = 1;
		throwVelocity.x = Math.Abs(throwVelocity.x);
	}

	public bool IsOnGround() 
	{
		return (groundDetectLeft.IsColliding() || groundDetectRight.IsColliding()) 
				&& Velocity.y >= 0;
	}

	public void SetColor(Color color) 
	{
		sprite.Modulate = color;
	}

	public Enemy GrabEnemy()
	{
		return throwDetect.GetCollider() as Enemy;
	}

	public void ThrowEnemy(Enemy enemy) 
	{
		enemy.IsGrabbed = false;
		enemy.IsThrown = true;
		enemy.Velocity = throwVelocity + Velocity;
	}

	public void ThrowDownEnemy(Enemy enemy)
	{
		enemy.IsGrabbed = false;
		enemy.IsThrown = false;
		enemy.IsThrownDown = true;
		enemy.Velocity = new Vector2(Velocity.x * 0.5f, throwDownSpeed);
	}

	public Bomb CreateBomb(String name) 
	{
		PackedScene bombLoader = ResourceLoader.Load<PackedScene>("res://Prefabs/Items/Bomb.tscn");
		Bomb bomb = bombLoader.Instance<Bomb>();
		bomb.Name = name;
		GetNode<Node2D>("/root/Testing").AddChild(bomb);
		bomb.Position = ThrowPoint;
		return bomb;
	}

	public void ThrowBomb() {
		if (State.Bomb == null) 
			return;

		GD.Print(State.ThrowForceMultiplier);
		// State.throwForceMultiplier * throwDirection * FaceDirection * maxThrowForce
		State.Bomb.ApplyCentralImpulse(maxThrowForce * throwDirection * FaceDirection * State.ThrowForceMultiplier);
		State.Bomb = null;
		State.ThrowForceMultiplier = 0;
	}
}
