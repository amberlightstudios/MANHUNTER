using Godot;
using System;

public class RushingEnemy : Enemy
{
	[Export]
	private float speed;
	[Export]
	private float explodeLoadTime = 0.6f;
	[Export]
	private float explodeRadius;

	private Goblin player = null;
	private bool isChasing = false, isExploding = false, isDead = false;

	private RayCast2D playerDetect, playerDetectBack;

	public override void _Ready()
	{
		playerDetect = GetNode<RayCast2D>("Sprite/PlayerDetect");
		playerDetectBack = GetNode<RayCast2D>("Sprite/PlayerDetectBack");
		sprite = GetNode<Sprite>("Sprite");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	private float explodeTimer = 0f;
	public override void _Process(float delta)
	{
		if (isDead) {
			if (!animPlayer.IsPlaying()) {
				GetParent().RemoveChild(this);
			}
			return;
		}

		if (player != null && (player.Position - Position).Length() < explodeRadius) {
			isExploding = true;
		}

		if (isExploding) {
			explodeTimer += delta;
			if (explodeTimer > explodeLoadTime) {

			}
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

		if ((playerDetect.IsColliding() && Goblin.PlayerType.Equals(playerDetect.GetCollider().GetType())) 
		|| (playerDetectBack.IsColliding() && Goblin.PlayerType.Equals(playerDetectBack.GetCollider().GetType()))
		|| isChasing) {
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

		if (isExploding) {
			velocity = new Vector2(0, velocity.y);
		}

		velocity.y += Gravity;
		MoveAndSlide(velocity);
	}

	public override void TurnLeft() 
	{
		sprite.Scale = new Vector2(Math.Abs(sprite.Scale.x), sprite.Scale.y);
	}

	public override void TurnRight() 
	{
		sprite.Scale = new Vector2(-1 * Math.Abs(sprite.Scale.x), sprite.Scale.y);
	}

	public override void Death() 
	{
		isDead = true;
		PlayAnimation("Death");
	}

	public override void PlayAnimation(string name)
	{
		base.PlayAnimation(name);
		if (isDead) {
			return;
		}
		if (isExploding) {
			
		}
	}

	public override void _Draw()
	{
		// DrawCircle(Vector2.Zero, explodeRadius, new Color(0, 0, 0));
	}
}
