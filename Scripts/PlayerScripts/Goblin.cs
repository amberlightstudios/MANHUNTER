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


	[Export]
	public float DashSpeed { get; private set; }


	[Export]
	private int meleeDamage = 2;
	[Export]
	public float AttakDeceleration = 0.5f;
	private Area2D meleeArea;


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
	private float knockBackSpeed;
	public float KnockBackSpeed { get => knockBackSpeed; }
	[Export]
	private int invincibleTime = 1300;
	[Export]
	private int stunTime = 200;
	public int StunTime { get => stunTime; }
	public bool IsInvincible = false;
	private bool stunAfterHit = false;



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

	
	private Area2D ladderDetection;
	[Export]
	private float ladderClimbSpeed;
	public float LadderClimbSpeed { get => ladderClimbSpeed; }

	private GameManager gm;
	private Vector2 defaultSpriteScale;
	
	private CPUParticles2D walk;
	public CPUParticles2D Walk { get => walk; }
	private CPUParticles2D jump;
	public CPUParticles2D Jump { get => jump; }

	public override void _Ready()
	{
		gm =  GetParent().GetNode<GameManager>("GameManager");
		gm.AddNewPlayer(this);
		
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		walkCollisionBox = GetNode<CollisionShape2D>("WalkCollsionBox");
		enemyHitBox = GetNode<Area2D>("EnemyHitBox");
		sprite = GetNode<Sprite>("Sprite");
		groundDetectLeft = GetNode<RayCast2D>("GroundDetectLeft");
		groundDetectRight = GetNode<RayCast2D>("GroundDetectRight");
		wallDetect = GetNode<RayCast2D>("WalkCollsionBox/WallDetect");
		WallDetectFoot = GetNode<RayCast2D>("WalkCollsionBox/WallDetectFoot");
		meleeArea = GetNode<Area2D>("Sprite/MeleeArea");
		ladderDetection = GetNode<Area2D>("LadderDetection");

		walk = GetNode<CPUParticles2D>("Particles/Walk");
		jump = GetNode<CPUParticles2D>("Particles/Jump");

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
				State._Process(delta);
				BroadcastState();
			}	
			else {
				ReceiveState();
			}
		} else {
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
		State._PhysicsProcess(delta);
		// Gravity
		Velocity.y += Gravity;
		Velocity = MoveAndSlide(Velocity);
		
		if (Velocity.y != 0) {
			walk.SetEmitting(false);
		}
	}

	public override void TakeDamage(int dmg) 
	{   
		if (IsInvincible || dmg == 0)
			return;
		base.TakeDamage(dmg);

		if (health <= 0) {
			State = new DeadState(this);
		}

		animPlayer.Play("Attacked");
		stunAfterHit = true;
		IsInvincible = true;
		Task.Delay(200).ContinueWith(t => stunAfterHit = false);
		Task.Delay(invincibleTime).ContinueWith(t => IsInvincible = false);
	}

	public void RestartGame() 
	{ 
		GetTree().ReloadCurrentScene(); 
	}
	
	public void GameOver()
	{
		GetTree().ChangeScene("res://Scenes/UI/GameOver.tscn");
	}

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
		}
	}

	public void TurnLeft() 
	{
		sprite.Position = new Vector2(1, 0);
		sprite.Scale = new Vector2(-defaultSpriteScale.x, defaultSpriteScale.y);
		FaceDirection = -1;
		throwVelocity.x = Math.Abs(throwVelocity.x) * -1;
		wallDetect.Scale = Vector2.One;
		WallDetectFoot.Scale = Vector2.One;
		walk.Position = new Vector2(3, 9);
	}

	public void TurnRight() 
	{
		sprite.Position = Vector2.Zero;
		sprite.Scale = defaultSpriteScale;
		FaceDirection = 1;
		throwVelocity.x = Math.Abs(throwVelocity.x);
		wallDetect.Scale = new Vector2(-1, 1);
		WallDetectFoot.Scale = new Vector2(-1, 1);
		walk.Position = new Vector2(-3, 9);
	}

	public bool OnGround() 
	{
		return (groundDetectLeft.IsColliding() || groundDetectRight.IsColliding()) 
				&& Velocity.y >= 0;
	}

	public bool OnLadder() 
	{
		return ladderDetection.GetOverlappingBodies().Count > 0;
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

	
	[Remote]
	public void SyncAttack()
	{
		AttackEnemy();
	}

	public bool AttackEnemy() 
	{
		if (GetTree().NetworkPeer != null && IsNetworkMaster()) Rpc(nameof(SyncAttack));
		Godot.Collections.Array enemiesInRange = meleeArea.GetOverlappingBodies();
		foreach (Enemy enemy in enemiesInRange) {
			Vector2 enemyPosition = enemy.Position;
			enemy.TakeDamage(meleeDamage, new Vector2(FaceDirection * 30f, 0));
		}
		return enemiesInRange.Count > 0;
	}

	public void PlayAnimation(String name) 
	{
		if (name == animPlayer.CurrentAnimation) {
			return;
		}
		animPlayer.Play(name);
		((GoblinSound)GetNode("SoundEffects")).PlaySound(name);
	}
}
