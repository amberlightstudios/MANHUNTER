using Godot;
using System;

public class Baby : Enemy
{
	[Export]
	private float speed;

	[Export]
	public float Gravity { get; private set; }

	private RayCast2D edgeDetectLeft, edgeDetectRight;
	private Sprite sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		edgeDetectLeft = GetNode<RayCast2D>("EdgeDetectLeft");
		edgeDetectRight = GetNode<RayCast2D>("EdgeDetectRight");
		sprite = GetNode<Sprite>("Sprite");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		
	}

	public override void _PhysicsProcess(float delta)
	{
		if (IsGrabbed) 
			return;

		if (velocity.y != 0) {
			if (IsThrown) {
				velocity.y += Gravity * 0.4f;
			} else {
				velocity.y += Gravity;
			}
			velocity = MoveAndSlide(velocity);
			return;
		}

		// What happens when the enemy is being thrown and just landed. 
		if (IsThrown) {
			IsThrown = false;
		}

		if (!edgeDetectLeft.IsColliding() && speed < 0) {
			speed *= -1;
		} else if (!edgeDetectRight.IsColliding() && speed > 0) {
			speed *= -1;
		}

		velocity = new Vector2(speed, velocity.y);

		velocity.y += Gravity;
		MoveAndSlide(velocity);
	}
}
