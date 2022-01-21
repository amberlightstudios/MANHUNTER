using Godot;
using System;
using GoblinStates;

public class Goblin : KinematicBody2D
{
	public GoblinState State;

	[Export]
	public int Health { get; private set; }

	[Export]
	public float Speed { get; private set; }

	public Vector2 Velocity;
	
	[Puppet]
	public Vector2 PuppetPosition { get; set; }
	[Puppet]
	public Vector2 PuppetVelocity { get; set; }

	[Export]
	public float JumpSpeed { get; private set; }

	[Export]
	public float Gravity { get; private set; }

	[Export]
	private float throwAngle = 0f;

	// faceDirection == -1 -> Player is facing left.. 
	// faceDirection == 1 -> Player is facing right. 
	private int faceDirection = -1;

	private AnimationPlayer animPlayer;
	public AnimationPlayer AnimPlayer { get => animPlayer; }
	private Sprite sprite;
	public Sprite PlayerSprite { get => sprite; }
	private CollisionShape2D walkCollisionBox;
	public CollisionShape2D WalkCollisionBox { get => walkCollisionBox; }

	private RayCast2D groundDetectLeft;
	private RayCast2D groundDetectRight;
	private Vector2 defaultSpriteScale;

	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		walkCollisionBox = GetNode<CollisionShape2D>("WalkCollsionBox");
		sprite = GetNode<Sprite>("Sprite");
		groundDetectLeft = GetNode<RayCast2D>("GroundDetectLeft");
		groundDetectRight = GetNode<RayCast2D>("GroundDetectRight");
		defaultSpriteScale = sprite.Scale;

		State = new MoveState(this);
	}

	public override void _Process(float delta)
	{
		var isMultiPlayer = GetTree().NetworkPeer != null;
		if (isMultiPlayer) {
			if (IsNetworkMaster()) {
				State._Process(delta);
				Rset(nameof(PuppetPosition), Position);			
				Rset(nameof(PuppetVelocity), Velocity);
				// Rpc("_UpdateState", PuppetPosition, PuppetVelocity);
			}	
			else {
				Position = PuppetPosition;
				Velocity = PuppetVelocity;
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
				// Gravity
				Velocity.y += Gravity;
				Velocity = MoveAndSlide(Velocity);
				Rset(nameof(PuppetPosition), Position);			
				Rset(nameof(PuppetVelocity), Velocity);
				// Rpc("_UpdateState", PuppetPosition, PuppetVelocity);
			}	
			else {
				Position = PuppetPosition;
				Velocity = PuppetVelocity;
			}
		} else {
		State._PhysicsProcess(delta);

		// Gravity
		Velocity.y += Gravity;
		Velocity = MoveAndSlide(Velocity);
		}
		

		if (isMultiPlayer && !IsNetworkMaster())
			PuppetPosition = Position;
	}

	public void TurnLeft() 
	{
		sprite.Position = Vector2.Zero;
		sprite.Scale = defaultSpriteScale;
		faceDirection = -1;
	}

	public void TurnRight() 
	{
		sprite.Position = new Vector2(-6, 0);
		sprite.Scale = new Vector2(-defaultSpriteScale.x, defaultSpriteScale.y);
		faceDirection = 1;
	}

	public bool IsOnGround() 
	{
		return (groundDetectLeft.IsColliding() || groundDetectRight.IsColliding()) 
				&& Velocity.y >= 0;
	}
}
