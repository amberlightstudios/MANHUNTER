using Godot;
using System;
using GoblinStates;
using System.Threading.Tasks;


public class Goblin : Character
{
	public GoblinState State;
	public static Type PlayerType = new Goblin().GetType();
	public string PlayerName;
	
	[Export]
	public float Speed { get; private set; }
	[Export]
	public float SpeedBoost { get; private set; }
	public Vector2 Velocity;
	public bool Killed = false;


	[Puppet] public Vector2 PuppetPosition { get; set; }
	[Puppet] public Vector2 PuppetVelocity { get; set; }
	[Puppet] public int PuppetFaceDirection { get; set; }
	[Puppet] public String PuppetAnimation { get; set; }
	[Puppet] public bool PuppetKilled { get; set; }
	[Puppet] public Color PuppetColor { get; set; }
	[Puppet] public bool PuppetIsRevived { get; set; }

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


	// This is for throwing. (need to delete this part later)
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

	public bool Invincible = false;

	public int FaceDirection { get; private set; }

	private AnimationPlayer animPlayer;
	public AnimationPlayer AnimPlayer { get => animPlayer; }
	private Sprite sprite;
	public Sprite PlayerSprite { get => sprite; }
	private CollisionShape2D walkCollisionBox;
	private Area2D enemyHitBox;

	private RayCast2D groundDetectLeft;
	private RayCast2D groundDetectRight;

	private RayCast2D wallDetect;
	public RayCast2D WallDetectFoot { get; private set; }

	[Export]
	private float ladderClimbSpeed;
	private Area2D ladderDetection;
	private RayCast2D ladderDetectTop, ladderDetectFoot, ladderDetectSide;
	public float LadderClimbSpeed { get => ladderClimbSpeed; }

	[Export]
	private float reviveTime = 3f;
	public float ReviveTime { get => reviveTime; }
	private bool isRevived = false;
	public bool IsRevived { 
		get => isRevived; 
		set {
			if (value) RevivePlayerPuppet();
		}    
	}
	private Area2D reviveDetect;

	public GameManager gm;
	public int PlayerIndex;
	private Vector2 defaultSpriteScale;
	
	private CPUParticles2D walk;
	public CPUParticles2D Walk { get => walk; }
	private CPUParticles2D jump;
	public CPUParticles2D Jump { get => jump; }

	private Vector2 screenSize;
	
	private Node2D nameTag;
	private Label name;
	
	public override void _Ready()
	{
		gm =  GetParent().GetNode<GameManager>("GameManager");
		PlayerIndex = gm.AddNewPlayer(this);
		
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
		ladderDetectTop = GetNode<RayCast2D>("LadderDetection/LadderDetectTop");
		ladderDetectFoot = GetNode<RayCast2D>("LadderDetection/LadderDetectFoot");
		ladderDetectSide = GetNode<RayCast2D>("LadderDetection/LadderDetectSide");

		reviveDetect = GetNode<Area2D>("ReviveArea");

		walk = GetNode<CPUParticles2D>("Particles/Walk");
		jump = GetNode<CPUParticles2D>("Particles/Jump");
		
		nameTag = GetNode<Node2D>("NameTag");
		name = GetNode<Label>("NameTag/Panel/Name");
		if (Globals.PlayerName != "") name.Text = Globals.PlayerName;
		else nameTag.Visible = false;

		defaultSpriteScale = sprite.Scale;
		FaceDirection = -1;

		normalGravity = Gravity;

		screenSize = GetViewport().GetVisibleRect().Size;

		State = new MoveState(this);
	}

	public override void _Process(float delta)
	{
		if (Globals.SinglePlayer || IsNetworkMaster()) {
			State._Process(delta);			
		}
		SynchronizeState();
		if (!Globals.SinglePlayer && !IsNetworkMaster())
			PuppetPosition = Position;
	}

	public override void _PhysicsProcess(float delta)
	{
		if (Globals.SinglePlayer || IsNetworkMaster()) {
			if (Position.y > screenSize.y + 50) {
				HandleDropDead();
			}

			State._PhysicsProcess(delta);
			if (Velocity.y < TERMINAL_VELOCITY) {
				Velocity.y += Gravity;
			}
			Velocity = MoveAndSlide(Velocity);
			
			if (Velocity.y != 0) {
				walk.Emitting = false;
			}
		}
		SynchronizeState();
	}
	
	public void SynchronizeState() {
		if (!Globals.SinglePlayer) {
			if (IsNetworkMaster()) { 
				BroadcastState();
			} else {
				Interpolate();
				ReceiveState();
			}
		} 
	}
	
	[Remote]
	public void UpdateState(Vector2 pos, Vector2 vel, int fd, string anim, 
							bool killed, Color color, bool ir)
	{
		PuppetPosition = pos;
		PuppetVelocity = vel;
		PuppetFaceDirection = fd;
		PuppetAnimation = anim;
		PuppetKilled = killed;
		PuppetColor = color;
		PuppetIsRevived = ir;
	}
	
	public void BroadcastState() 
	{
		RpcUnreliable(nameof(UpdateState), Position, Velocity, FaceDirection, 
		animPlayer.CurrentAnimation, Killed, sprite.Modulate, isRevived);
	}
	
	public void ReceiveState() 
	{
		Position = PuppetPosition;
		Velocity = PuppetVelocity;
		if (sprite.Modulate != PuppetColor)
			SetColor(PuppetColor);
		if (FaceDirection == -1 && PuppetFaceDirection == 1) {
			TurnRight();
		} else if (FaceDirection != PuppetFaceDirection) {
			TurnLeft();
		}  
		if (PuppetAnimation != null) {
			animPlayer.Play(PuppetAnimation);
		}
		if (PuppetKilled) {
			if (!Killed) {
				SetDead();
			} 
		} else {
			if (Killed) {
				SetCollisionLayerBit(1, true);
				SetCollisionLayerBit(7, false);
				enemyHitBox.SetCollisionLayerBit(1, true);
				enemyHitBox.SetCollisionLayerBit(7, false);
			}
		}
		Killed = PuppetKilled;
		if (PuppetIsRevived) {
			if (!isRevived) RevivePlayer();
		}
		isRevived = PuppetIsRevived;
		
	}
	
	public void Interpolate() 
	{
		if (Velocity.y < TERMINAL_VELOCITY) {
			Velocity.y += Gravity;
		}
		Velocity = MoveAndSlide(Velocity);
	}
	
	public void HandleDropDead()
	{
		Killed = true;
		SynchronizeState();
		RemoveSelf();
	}
	
	public void SetDead()
	{
		gm.RemovePlayer(PlayerIndex);
		if (gm.NumPlayers == 0) GameOver();
		SetCollisionLayerBit(1, false);
		SetCollisionLayerBit(7, true);
		enemyHitBox.SetCollisionLayerBit(1, false);
		enemyHitBox.SetCollisionLayerBit(7, true);
		// SetColor(new Color( 1, 0, 0, 1 ));
	}

	public void RemoveSelf() {
		FreeCamera();				
		gm.RemovePlayer(PlayerIndex);
		if (gm.NumPlayers == 0) GameOver();			
		else {
			AttachCamera();			
			QueueFree();
		} 
	}
	
	public void FreeCamera() {
		 Cam cam = (Cam) GetParent().GetNode("Cam");
		if (cam.Player == this)
			 cam.Player = null;
	}
	
	public void AttachCamera() {
		Cam cam = (Cam) GetParent().GetNode("Cam");
		cam.Player = gm.GetRandomAlive();
	}

	public override void TakeDamage(int dmg) 
	{   
		if (Killed || Invincible) return;
		base.TakeDamage(dmg);
		if (health <= 0)
			State = new DeadState(this);
	}

	public Goblin FindReviveTarget() 
	{
		Godot.Collections.Array targets = reviveDetect.GetOverlappingBodies();
		foreach (Goblin g in targets) {
			return g;
		}
		return null;
	}

	public void RestartGame() 
	{ 
		GetTree().ReloadCurrentScene(); 
	}
	
	public void GameOver()
	{
		if (!Globals.SinglePlayer) {
			((Network) GetParent()).LeaveGame();
		} else {
			GetTree().ChangeScene("res://Scenes/UI/GameOver.tscn");
		}
	}

	async Task HandleDeathAnim() 
	{
		animPlayer.Play("Death");
		await Task.Delay(885);
		animPlayer.Play("Ghost");
		await Task.Delay(2840);
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
	

	public void TurnLeft() 
	{
		sprite.Position = new Vector2(1, 0);
		sprite.Scale = new Vector2(-defaultSpriteScale.x, defaultSpriteScale.y);
		FaceDirection = -1;
		throwVelocity.x = Math.Abs(throwVelocity.x) * -1;
		wallDetect.Scale = Vector2.One;
		WallDetectFoot.Scale = Vector2.One;
		ladderDetectSide.Scale = new Vector2(-1, 1);
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
		ladderDetectSide.Scale = Vector2.One;
		walk.Position = new Vector2(-3, 9);
	}

	public bool OnGround() 
	{
		return (groundDetectLeft.IsColliding() 
			|| groundDetectRight.IsColliding()) 
				&& Velocity.y >= 0;
	}

	public void SetLadderCollision(bool activate) 
	{
		groundDetectLeft.SetCollisionMaskBit(6, activate);
		groundDetectRight.SetCollisionMaskBit(6, activate);
		SetCollisionLayerBit(9, activate);
	}

	public bool OnLadder() 
	{
		return ladderDetection.GetOverlappingBodies().Count > 0;
	}

	public bool IsRunningIntoLadder() 
	{
		return ladderDetectSide.IsColliding();
	}

	public bool IsFallingTowardsLadder() 
	{   
		return !ladderDetectTop.IsColliding() && ladderDetectFoot.IsColliding() && Velocity.y > 200;
	}

	public bool IsHittingLadderOnTop() 
	{
		return ladderDetectTop.IsColliding();
	}

	public bool CanWallClimb() 
	{
		return wallDetect.IsColliding();
	}

	public void SetZeroGravity() 
	{
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

	public bool AttackEnemy() 
	{
		Godot.Collections.Array enemiesInRange = meleeArea.GetOverlappingBodies();
		foreach (Enemy enemy in enemiesInRange) {
			Vector2 enemyPosition = enemy.Position;
			enemy.TakeDamage(meleeDamage, new Vector2(FaceDirection * 30f, 0));
		}
		return enemiesInRange.Count > 0;
	}

	public void DeflectBullet() 
	{
		Godot.Collections.Array bulletsHit = meleeArea.GetOverlappingAreas();
		foreach (Bullet bullet in bulletsHit) {
			bullet.Deflect();
		}
	}

	public void PlayAnimation(String name) 
	{
		if (name == animPlayer.CurrentAnimation) {
			return;
		}
		if (name == "Walk") {
			walk.Emitting = true;
		} else {
			walk.Emitting = false;
		}
		animPlayer.Play(name);
		((GoblinSound)GetNode("SoundEffects")).PlaySound(name);
	}
	
	public void SetName(String name)
	{
		PlayerName = name;
		Label nameTag = (Label) GetNode("NameTag/Panel/Name");
		nameTag.Text = name;
	}
	
	public void RevivePlayer()
	{
		isRevived = true;
		Killed = false;
		State.ExitState(new MoveState(this));
		gm.SetNewPlayer(this, PlayerIndex);
		SetCollisionLayerBit(1, true);
		SetCollisionLayerBit(7, false);
		enemyHitBox.SetCollisionLayerBit(1, true);
		enemyHitBox.SetCollisionLayerBit(7, false);
		SetColor(new Color(1, 1, 1, 1));
		return;
	}
	
	public void RevivePlayerPuppet()
	{
		Rpc(nameof(RevivePlayerMaster));
	}
	
	[Remote]
	public void RevivePlayerMaster()
	{
		if (IsNetworkMaster()) RevivePlayer();
	}
}
