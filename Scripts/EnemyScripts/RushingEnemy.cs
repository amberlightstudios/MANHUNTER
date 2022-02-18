using Godot;
using System;
using RushEnemyStates;

public class RushingEnemy : Enemy
{
	[Export]
	private float chaseSpeed;
	public float ChaseSpeed { get => chaseSpeed; }
	[Export]
	private float roamSpeed;
	public float RoamSpeed { get => roamSpeed; }

	private RayCast2D playerDetect, playerDetectBack, edgeDetectLeft, edgeDetectRight, wallDetect;
	private Area2D meleeArea;

	public RushEnemyState State;

	public override void _Ready()
	{
		playerDetect = GetNode<RayCast2D>("Sprite/PlayerDetect");
		playerDetectBack = GetNode<RayCast2D>("Sprite/PlayerDetectBack");
		meleeArea = GetNode<Area2D>("Sprite/MeleeArea");
		edgeDetectLeft = GetNode<RayCast2D>("Sprite/EdgeDetectLeft");
		edgeDetectRight = GetNode<RayCast2D>("Sprite/EdgeDetectRight");
		wallDetect = GetNode<RayCast2D>("Sprite/WallDetect");
		sprite = GetNode<Sprite>("Sprite");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		State = new NormalState(this);
	}

	private float attackTimer = 0f;
	public override void _Process(float delta)
	{
		State._Process(delta);
	}

	public override void _PhysicsProcess(float delta)
	{
		State._PhysicsProcess(delta);

		base._PhysicsProcess(delta);
	}

	public bool EdgeDetect() 
	{
		if ((!edgeDetectLeft.IsColliding() || wallDetect.IsColliding()) && velocity.x < 0) {
			TurnRight();
			velocity.x *= -1;
			return true;
		} else if ((!edgeDetectRight.IsColliding() || wallDetect.IsColliding()) && velocity.x > 0) {
			TurnLeft();
			velocity.x *= -1;
			return true;
		}

		return false;
	}

	public Goblin PlayerDetect() 
	{
		if (playerDetect.IsColliding() && Goblin.PlayerType.Equals(playerDetect.GetCollider().GetType())) {
			return (Goblin) playerDetect.GetCollider();
		}
		if (playerDetectBack.IsColliding() && Goblin.PlayerType.Equals(playerDetectBack.GetCollider().GetType())) {
			return (Goblin) playerDetectBack.GetCollider();
		}

		return null;
	}

	public bool PlayerInRange() 
	{
		return meleeArea.GetOverlappingAreas().Count > 0;
	}

	public override void Death() 
	{
		State.ExitState(new DeathState(this));
	}

	public void Attack() 
	{
		foreach (Area2D g in meleeArea.GetOverlappingAreas()) {
			((Goblin) g.GetParent()).TakeDamage(5);
		}
	}

	public override void PlayAnimation(string name)
	{
		if (name == animPlayer.CurrentAnimation) {
			return;
		}
		base.PlayAnimation(name);
	}

	public override void _Draw()
	{
		// DrawCircle(Vector2.Zero, explodeRadius, new Color(0, 0, 0));
	}
}
