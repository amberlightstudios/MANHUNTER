using Godot;
using System;
using GoblinStates;

public class Goblin : KinematicBody2D
{
	public GoblinState State;

	[Export]
	public float Speed { get; private set; }

	public Vector2 Velocity;

	[Export]
	public float JumpSpeed { get; private set; }

	[Export]
	public float Gravity { get; private set; }
	
	private Label NameLabel { get; set; }

	[Puppet]
	public Vector2 PuppetPosition { get; set; }
	[Puppet]
	public Vector2 PuppetVelocity { get; set; }

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
		var isMultiPlayer = GetTree().IsNetworkServer();
		if (isMultiPlayer) {
			if (IsNetworkMaster()) {
				GD.Print("Master");
				State._Process(delta);
				Rset(nameof(PuppetPosition), Position);
				Rset(nameof(PuppetVelocity), Velocity);
			}	else {
				GD.Print("Client");
				Position = PuppetPosition;
				Velocity = PuppetVelocity;
			}
		} else {
			State._Process(delta);	
		}
		

	}

	public override void _PhysicsProcess(float delta)
	{
		State._PhysicsProcess(delta);

		// Gravity
		Velocity.y += Gravity;
		Velocity = MoveAndSlide(Velocity);
		
		if (!IsNetworkMaster()) {
			PuppetPosition = Position;
		}
	}

	public void TurnLeft() 
	{
		sprite.Position = Vector2.Zero;
		sprite.Scale = Vector2.One;
	}

	public void TurnRight() 
	{
		sprite.Position = new Vector2(-7, 0);
		sprite.Scale = new Vector2(-1, 1);
	}

	public bool IsOnGround() 
	{
		return (groundDetectLeft.IsColliding() || groundDetectRight.IsColliding()) 
				&& Velocity.y >= 0;
	}
	
	public void SetPlayerName(string name)
	{
		NameLabel = (Label)GetNode("Label");

		PuppetPosition = Position;
		PuppetVelocity = Velocity;

		NameLabel.Text = name;
	}
}
