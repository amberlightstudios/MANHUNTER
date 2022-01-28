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
	public float Gravity { get; private set; }
	[Export]
	public float attackRange { get; private set; }

	private RayCast2D edgeDetectLeft, edgeDetectRight, wallDetect;
	private Sprite sprite;
	private GameManager gamemanager;
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
		gamemanager = GetParent().GetNode<GameManager>("GameManager");

		State = new MoveState(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		// For debug drawing in _Draw
		Update();
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

		if (isTakingDamage)
			return;

		State._PhysicsProcess(delta);

		velocity.y += Gravity;
		MoveAndSlide(velocity);
	}

	public bool OnGround() 
	{
		return edgeDetectLeft.IsColliding() || edgeDetectRight.IsColliding();
	}

	public void CheckEdge() 
	{
		if ((!edgeDetectLeft.IsColliding() || wallDetect.IsColliding()) && velocity.x < 0) {
			TurnRight();
		} else if ((!edgeDetectRight.IsColliding() || wallDetect.IsColliding()) && velocity.x > 0) {
			TurnLeft();
		}
	}

	private void TurnLeft() 
	{
		velocity.x = Math.Abs(velocity.x) * -1;
		Speed = Math.Abs(Speed) * -1;
		sprite.Scale = new Vector2(-1, 1);
		GD.Print(State);
		((MoveState) State).IsChasing = false;
	}

	private void TurnRight() 
	{
		velocity.x = Math.Abs(velocity.x);
		Speed = Math.Abs(Speed);
		sprite.Scale = Vector2.One;
		((MoveState) State).IsChasing = false;
	}

	public bool PlayerInAttackRange() 
	{
		return gamemanager.Player.Position.x - Position.x < attackRange;
	}

	public float GetAttackDist() 
	{
		return gamemanager.Player.Position.x - Position.x;
	}

	public override void _Draw()
	{
		DrawLine(Vector2.Zero, new Vector2(attackRange, 0), new Color(0, 0, 0, 1));
	}
}
