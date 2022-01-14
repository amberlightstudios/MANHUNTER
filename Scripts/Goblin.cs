using Godot;
using System;
using GoblinStates;

public class Goblin : KinematicBody2D
{
	public GoblinState State;

	[Export]
	private float speed;
	public float Speed { get => speed; }

	[Export]
	private float jumpSpeed;
	public float JumpSpeed { get => jumpSpeed; }

	[Export]
	private float gravity = 1;
	public float Gravity { get => gravity; }

	private AnimationPlayer animPlayer;
	public AnimationPlayer AnimPlayer { get => animPlayer; }
	private Sprite sprite;
	public Sprite PlayerSprite { get => sprite; }
	private CollisionShape2D walkCollisionBox;
	public CollisionShape2D WalkCollisionBox { get => walkCollisionBox; }

	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		walkCollisionBox = GetNode<CollisionShape2D>("WalkCollsionBox");
		sprite = GetNode<Sprite>("Sprite");

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
		State.Velocity += new Vector2(0, gravity);
		State.Velocity = MoveAndSlide(State.Velocity);
	}
}
