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
		PuppetPosition = Position;
		State = new MoveState(this);
		
	}
	
	[Remote]
	public void _UpdateState(Vector2 pos, Vector2 vel) {
		GD.Print($"UPdating Client To Be ${pos}, ${vel}");
		PuppetPosition = pos;
		PuppetVelocity = vel;
	}

	public override void _Process(float delta)
	{

	}

	public override void _PhysicsProcess(float delta)
	{
		var isMultiPlayer = GetTree().NetworkPeer != null;
		if (isMultiPlayer) {
			if (IsNetworkMaster()) {
			State._PhysicsProcess(delta);
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
		}
		
		Velocity.y += Gravity;
		Velocity = MoveAndSlide(Velocity);

		if (isMultiPlayer && !IsNetworkMaster())
			PuppetPosition = Position;
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
		//NameLabel = (Label)GetNode("Label");

		PuppetPosition = Position;
		PuppetVelocity = Velocity;

		//NameLabel.Text = name;
	}
}
