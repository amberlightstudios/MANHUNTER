using Godot;
using System;
using RushEnemyStates;

public class RushingEnemy : Enemy
{
	[Export]
	private float speed;
	[Export]
	private float roamSpeed;
	public float RoamSpeed { get => roamSpeed; }

	private Goblin player = null;
	private bool isChasing = false, isAttack = false, isDead = false;

	private RayCast2D playerDetect, playerDetectBack;
	private Area2D meleeArea;

	public RushEnemyState State;

	public override void _Ready()
	{
		playerDetect = GetNode<RayCast2D>("Sprite/PlayerDetect");
		playerDetectBack = GetNode<RayCast2D>("Sprite/PlayerDetectBack");
		meleeArea = GetNode<Area2D>("Sprite/MeleeArea");
		sprite = GetNode<Sprite>("Sprite");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	private float attackTimer = 0f;
	public override void _Process(float delta)
	{
		if (isDead) {
			if (!animPlayer.IsPlaying()) {
				GetParent().RemoveChild(this);
			}
			return;
		}


		if (isChasing) {
			PlayAnimation("Walk");
		} else {
			PlayAnimation("Idle");
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (isDead)
			return;
			
		if (meleeArea.GetOverlappingBodies().Count > 0 && !isAttack) {
			isAttack = true;
			velocity.x = 0;
		}

		if ((playerDetect.IsColliding() && Goblin.PlayerType.Equals(playerDetect.GetCollider().GetType())) 
		|| (playerDetectBack.IsColliding() && Goblin.PlayerType.Equals(playerDetectBack.GetCollider().GetType()))) {
			if (!isChasing) {
				if (playerDetect.IsColliding()) {
					player = (Goblin) playerDetect.GetCollider();
				} else {
					player = (Goblin) playerDetectBack.GetCollider();
				}
			}

			if (!isChasing) {
				float attackDirection = Math.Sign(player.Position.x - Position.x);
				velocity = new Vector2(attackDirection * speed, velocity.y);
				if (velocity.x < 0) {
					TurnLeft();
				} else if (velocity.x > 0) {
					TurnRight();
				}
			}

			isChasing = true;
		} else {
			velocity = new Vector2(0, velocity.y);
		}

		velocity.y += Gravity;
		MoveAndSlide(velocity);
	}

	public bool PlayerDetect() {
		return (playerDetect.IsColliding() && Goblin.PlayerType.Equals(playerDetect.GetCollider().GetType())) 
		|| (playerDetectBack.IsColliding() && Goblin.PlayerType.Equals(playerDetectBack.GetCollider().GetType()));
	}

	public override void Death() 
	{
		isDead = true;
		// PlayAnimation("Death");
	}

	public override void PlayAnimation(string name)
	{
		if (name == animPlayer.CurrentAnimation) {
			return;
		}
		base.PlayAnimation(name);
		if (isDead) {
			return;
		}
	}

	public override void _Draw()
	{
		// DrawCircle(Vector2.Zero, explodeRadius, new Color(0, 0, 0));
	}
}
