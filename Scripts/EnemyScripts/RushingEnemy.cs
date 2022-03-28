using Godot;
using System;
using RushEnemyStates;
using Effect;

public class RushingEnemy : Enemy
{
	[Export]
	private float chaseSpeed;
	public float ChaseSpeed { get => chaseSpeed; }
	[Export]
	private float roamSpeed;
	public float RoamSpeed { get => roamSpeed; }
	[Export]
	private float knockBackSpeed;

	private RayCast2D playerDetect, playerDetectBack;
	private RayCast2D edgeDetectLeft, edgeDetectRight, wallDetect, groundDetect, ladderDetectSide;
	private Area2D meleeArea;
	public RushEnemyState State;

	public override void _Ready()
	{
		InitEnemy();
		playerDetect = GetNode<RayCast2D>("Sprite/PlayerDetect");
		playerDetectBack = GetNode<RayCast2D>("Sprite/PlayerDetectBack");
		meleeArea = GetNode<Area2D>("Sprite/MeleeArea");
		edgeDetectLeft = GetNode<RayCast2D>("EdgeDetectLeft");
		edgeDetectRight = GetNode<RayCast2D>("EdgeDetectRight");
		groundDetect = GetNode<RayCast2D>("GroundDetect");
		wallDetect = GetNode<RayCast2D>("Sprite/WallDetect");
		ladderDetectSide = GetNode<RayCast2D>("Sprite/LadderDetectSide");
		sprite = GetNode<Sprite>("Sprite");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		State = new NormalState(this);
	}

	private float attackTimer = 0f;
	public override void _Process(float delta)
	{
		if (GetTree().NetworkPeer == null || GetTree().IsNetworkServer()) {
			State._Process(delta);		
		} 
		SynchronizeState();	
	}

	public override void _PhysicsProcess(float delta)
	{
		if (GetTree().NetworkPeer == null || GetTree().IsNetworkServer()) {
			State._PhysicsProcess(delta);

			if (ladderDetectSide.IsColliding()) {
				SetCollisionLayerBit(9, false);
			} else {
				SetCollisionLayerBit(9, true);
			}

			if (!OnGround()) {
				velocity.y += Gravity;
			}
			MoveAndSlide(velocity);
		}  
		SynchronizeState();
	}

	public bool OnGround() 
	{
		return groundDetect.IsColliding();
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

	public override int Attack() 
	{
		foreach (Area2D g in meleeArea.GetOverlappingAreas()) {
			Goblin player = g.GetParent<Goblin>();
			if (!player.IsAttacking) {
				player.TakeDamage(5);
			} else {
				return -1;
			}
		}

		return 0;
	}

	public override void KnockBack() 
	{
		velocity.x = -1 * FaceDirection * knockBackSpeed;
		State.ExitState(new KnockBackState(this));
	}
	
	public override void TakeDamage(int dmg, Vector2 knockbackDist)
	{
		if (!Globals.SinglePlayer)
			Rpc(nameof(TakeDamageRemote), dmg, knockbackDist);
		base.TakeDamage(dmg, knockbackDist);
	}
	
	[Remote]
	public void TakeDamageRemote(int dmg, Vector2 knockbackDist)
	{
		base.TakeDamage(dmg, knockbackDist);
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
