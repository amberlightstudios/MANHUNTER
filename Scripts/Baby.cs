using Godot;
using System;
using BabyStates;

public class Baby : Enemy
{
	[Export]
	public float Speed { get; private set; }
	[Export]
	public float ChaseMultiplier { get; private set; }

	[Export]
	public float Gravity { get; private set; }

	private RayCast2D edgeDetectLeft, edgeDetectRight, wallDetect;
	private Sprite sprite;
	public RayCast2D PlayerDetect { get; private set; }

	public BabyState State;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		edgeDetectLeft = GetNode<RayCast2D>("EdgeDetectLeft");
		edgeDetectRight = GetNode<RayCast2D>("EdgeDetectRight");
		wallDetect = GetNode<RayCast2D>("Sprite/WallDetect");
		PlayerDetect = GetNode<RayCast2D>("Sprite/PlayerDetect");
		sprite = GetNode<Sprite>("Sprite");

		State = new MoveState(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		
	}

	public override void _PhysicsProcess(float delta)
	{
		// if (IsGrabbed) 
		// 	return;

		// if (velocity.y != 0) {
		// 	// if (IsThrown) {
		// 	// 	velocity.y += Gravity * 0.4f;
		// 	// } else {
		// 	velocity.y += Gravity;
		// 	// }
		// 	velocity = MoveAndSlide(velocity);
		// 	return;
		// }

		// // What happens when the enemy is being thrown and just landed. 
		// if (IsThrown) {
		// 	IsThrown = false;
		// }

		State._PhysicsProcess(delta);

		if ((!edgeDetectLeft.IsColliding() || wallDetect.IsColliding()) && velocity.x < 0) {
			TurnRight();
		} else if ((!edgeDetectRight.IsColliding() || wallDetect.IsColliding()) && velocity.x > 0) {
			TurnLeft();
		}

		velocity.y += Gravity;
		MoveAndSlide(velocity);
	}

	private void TurnLeft() 
	{
		velocity.x = Math.Abs(velocity.x) * -1;
		Speed = Math.Abs(Speed) * -1;
		sprite.Scale = new Vector2(-1, 1);
	}

	private void TurnRight() 
	{
		velocity.x = Math.Abs(velocity.x);
		Speed = Math.Abs(Speed);
		sprite.Scale = Vector2.One;
	}
}
