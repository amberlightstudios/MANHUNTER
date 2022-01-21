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

	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		walkCollisionBox = GetNode<CollisionShape2D>("WalkCollsionBox");
		sprite = GetNode<Sprite>("Sprite");
		groundDetectLeft = GetNode<RayCast2D>("GroundDetectLeft");
		groundDetectRight = GetNode<RayCast2D>("GroundDetectRight");

		State = new MoveState(this);
	}

	public override void _Process(float delta)
	{
		State._Process(delta);
	}

	public override void _PhysicsProcess(float delta)
	{
		State._PhysicsProcess(delta);

		// Gravity
		Velocity.y += Gravity;
		Velocity = MoveAndSlide(Velocity);
	}

	public void TurnLeft() 
	{
		sprite.Position = Vector2.Zero;
		sprite.Scale = Vector2.One;
		faceDirection = -1;
	}

	public void TurnRight() 
	{
		sprite.Position = new Vector2(-7, 0);
		sprite.Scale = new Vector2(-1, 1);
		faceDirection = 1;
	}

	public bool IsOnGround() 
	{
		return (groundDetectLeft.IsColliding() || groundDetectRight.IsColliding()) 
				&& Velocity.y >= 0;
	}
}
