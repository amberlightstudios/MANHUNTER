using Godot;
using System;
using GoblinStates;
using System.Threading.Tasks;


public class Goblin : Character
{
	public GoblinState State;
	public static Type PlayerType = new Goblin().GetType();

	[Export]
	public float Speed { get; private set; }
	public Vector2 Velocity;
	
	[Export]
	private int meleeDamage = 2;

	[Puppet]
	public Vector2 PuppetPosition { get; set; }
	[Puppet]
	public Vector2 PuppetVelocity { get; set; }
	[Puppet]
	public int PuppetFaceDirection { get; set; }
	[Puppet]
	public String PuppetAnimation { get; set; }

	[Export]
	public float JumpSpeed { get; private set; }
	[Export]
	public float WallClimbSpeed { get; private set; }

	

	// This is for throwing.
	[Export]
	private int rocksCount = 4; 
	public int RocksCount { get => rocksCount; private set => rocksCount = value; }
	[Export]
	private Vector2 throwDirection;
	[Export]
	private float maxThrowForce = 3000f;
	[Export]
	public float LowThrowMultiplier = 0.5f;
	[Export]
	public float AdditionalThrowMultiplier = 0.1f;
	// This is for throwing enemies. (Temporary disable)
	[Export]
	private Vector2 throwVelocity;
	[Export]
	private float throwDownSpeed;
	[Export]
	private int invincibleTime = 1300;
	private bool isInvincible = false;

	// faceDirection == -1 -> Player is facing left.. 
	// faceDirection == 1 -> Player is facing right. 
	public int FaceDirection { get; private set; }

	private AnimationPlayer animPlayer;
	public AnimationPlayer AnimPlayer { get => animPlayer; }
	private Sprite sprite;
	public Sprite PlayerSprite { get => sprite; }
	private CollisionShape2D walkCollisionBox;
	public CollisionShape2D WalkCollisionBox { get => walkCollisionBox; }
	private Area2D enemyHitBox;

	private RayCast2D groundDetectLeft;
	private RayCast2D groundDetectRight;
	public Vector2 ThrowPoint { 
		get => (GetNode<Node2D>("Sprite/ThrowPoint").Position) * sprite.Scale + sprite.Position + Position;
		private set => ThrowPoint = value; }
	public Vector2 ThrowPointScale {
		get => GetNode<Node2D>("Sprite/ThrowPoint").Scale * sprite.Scale * Scale;
		private set => ThrowPointScale = value;
	}
	private string throwObjectPath = "res://Prefabs/Items/Rock.tscn";
	private RayCast2D wallDetect;
	public RayCast2D WallDetectFoot { get; private set; }

	private Area2D meleeArea;

	private Vector2 defaultSpriteScale;

	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		walkCollisionBox = GetNode<CollisionShape2D>("WalkCollsionBox");
		enemyHitBox = GetNode<Area2D>("EnemyHitBox");
		sprite = GetNode<Sprite>("Sprite");
		groundDetectLeft = GetNode<RayCast2D>("GroundDetectLeft");
		groundDetectRight = GetNode<RayCast2D>("GroundDetectRight");
		wallDetect = GetNode<RayCast2D>("WalkCollsionBox/WallDetect");
		WallDetectFoot = GetNode<RayCast2D>("WalkCollsionBox/WallDetectFoot");
		meleeArea = GetNode<Area2D>("Sprite/MeleeArea");
		defaultSpriteScale = sprite.Scale;
		FaceDirection = -1;

		State = new MoveState(this);
	}

	public override void _Process(float delta)
	{
		// Networking part
		var isMultiPlayer = GetTree().NetworkPeer != null;
		if (isMultiPlayer) {
			if (IsNetworkMaster()) {
				if (animPlayer.CurrentAnimation != "Attacked")
					State._Process(delta);
				BroadcastState();
			}	
			else {
				ReceiveState();
			}
		} else {
			if (animPlayer.CurrentAnimation == "Attacked") {
				return;
			}

			State._Process(delta);
		}
		

		if (isMultiPlayer && !IsNetworkMaster())
			PuppetPosition = Position;
	}

	public override void _PhysicsProcess(float delta)
	{
		var isMultiPlayer = GetTree().NetworkPeer != null;
		if (isMultiPlayer) {
			if (IsNetworkMaster()) {
				UpdateGoblin(delta);
				BroadcastState();
			}	
			else {
				ReceiveState();
			}
		}
		// Single player mode.  
		else {
			UpdateGoblin(delta);
		}
	}
	
	private void UpdateGoblin(float delta)
	{
		if (animPlayer.CurrentAnimation == "Attacked") {
			Velocity = Vector2.Zero;
		} else {
			State._PhysicsProcess(delta);
			// Gravity
			Velocity.y += Gravity;
			Velocity = MoveAndSlide(Velocity);
		}

	}

	public override void TakeDamage(int dmg) 
	{   
		if (animPlayer.CurrentAnimation == "Attacked" || isInvincible)
			return;
		base.TakeDamage(dmg);

		if (health <= 0) {
			State = new DeadState(this);
		}

		animPlayer.Play("Attacked");
		isInvincible = true;
		Task.Delay(invincibleTime).ContinueWith(t => isInvincible = false);
	}

	public void RestartGame() { GetTree().ReloadCurrentScene(); }

	public void Throw() 
	{
		if (animPlayer.CurrentAnimation == "Throw")
			return;

		if (rocksCount <= 0) {
			State.ExitState(null);
			return;
		}
		
		animPlayer.Play("Throw");
	}

	// When the throw animation ends and the player throws out the rock (or other objects). 
	public void GenerateRock() 
	{
		if (GetTree().NetworkPeer != null && IsNetworkMaster()) Rpc(nameof(SyncGenerateRock));
		PackedScene throwLoader = ResourceLoader.Load<PackedScene>("res://Prefabs/Items/Rock.tscn");
		Rock rock = throwLoader.Instance<Rock>();
		rock.Direction = FaceDirection;
		GetParent().AddChild(rock);
		rock.Position = ThrowPoint;
		rocksCount -= 1;
	}
	
	[Remote]
	public void SyncGenerateRock()
	{
		GenerateRock();
	}
	
	public void BroadcastState() 
	{
		Rset(nameof(PuppetPosition), Position);			
		Rset(nameof(PuppetVelocity), Velocity);
		Rset(nameof(PuppetFaceDirection), FaceDirection);
		Rset(nameof(PuppetAnimation), animPlayer.CurrentAnimation);	
	}
	
	public void ReceiveState() 
	{
		Position = PuppetPosition;
		Velocity = PuppetVelocity;
		if (FaceDirection == -1 && PuppetFaceDirection == 1) {
			TurnRight();
		} else if (FaceDirection != PuppetFaceDirection) {
			TurnLeft();
		}  
		if (PuppetAnimation != null) {
			animPlayer.Play(PuppetAnimation);
		} else {
			animPlayer.Play("Idle");
		}
	}

	public void TurnLeft() 
	{
		sprite.Position = Vector2.Zero;
		sprite.Scale = defaultSpriteScale;
		FaceDirection = -1;
		throwVelocity.x = Math.Abs(throwVelocity.x) * -1;
		wallDetect.Scale = Vector2.One;
		WallDetectFoot.Scale = Vector2.One;
	}

	public void TurnRight() 
	{
		sprite.Scale = new Vector2(-defaultSpriteScale.x, defaultSpriteScale.y);
		FaceDirection = 1;
		throwVelocity.x = Math.Abs(throwVelocity.x);
		wallDetect.Scale = new Vector2(-1, 1);
		WallDetectFoot.Scale = new Vector2(-1, 1);
	}

	public bool IsOnGround() 
	{
		return (groundDetectLeft.IsColliding() || groundDetectRight.IsColliding()) 
				&& Velocity.y >= 0;
	}

	public bool CanWallClimb() 
	{
		return wallDetect.IsColliding();
	}

	private float normalGravity;
	public void SetZeroGravity() 
	{
		normalGravity = Gravity;
		Gravity = 0f;
	}

	public void ReturnNormalGravity() 
	{
		Gravity = normalGravity;
	}

	public void SetColor(Color color) 
	{
		sprite.Modulate = color;
	}

	public void ThrowEnemy(Enemy enemy) 
	{
		enemy.IsGrabbed = false;
		enemy.IsThrown = true;
		enemy.Velocity = throwVelocity + Velocity;
	}

	public void ThrowDownEnemy(Enemy enemy)
	{
		enemy.IsGrabbed = false;
		enemy.IsThrown = false;
		enemy.IsThrownDown = true;
		enemy.Velocity = new Vector2(Velocity.x * 0.5f, throwDownSpeed);
	}

	// public Bomb CreateBomb(String name) 
	// {
		// PackedScene bombLoader = ResourceLoader.Load<PackedScene>("res://Prefabs/Items/Bomb.tscn");
		// Bomb bomb = bombLoader.Instance<Bomb>();
		// bomb.Name = name;
		// GetParent().AddChild(bomb);
		// bomb.Position = ThrowPoint;
	// 	return bomb;
	// }

	

	public void AttackEnemy() 
	{
		Godot.Collections.Array enemiesInRange = meleeArea.GetOverlappingBodies();
		foreach (Enemy enemy in enemiesInRange) {
			Vector2 enemyPosition = enemy.Position;
			enemy.TakeDamage(meleeDamage, new Vector2(FaceDirection * 30f, 0));
		}
	}
}
