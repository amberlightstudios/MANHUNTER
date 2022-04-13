using Godot;
using System;
using GoblinStates;
using System.Threading.Tasks;


public class Goblin : Character
{
	public GoblinState State;
	public static Type PlayerType = new Goblin().GetType();
	public string PlayerName;
	public Network NetworkNode;
	
	[Export]
	public int Lives = 3;
	[Export]
	public float Speed { get; private set; }
	[Export]
	public float SpeedBoost { get; private set; }
	public Vector2 Velocity;
	public bool IsDeadRevivable = false; // can still revive
	public bool IsDead = false; // cannot revive
	public bool IsTeamResetting = false;
	bool deathPlaying = false;

	[Puppet] public Vector2 PuppetPosition { get; set; }
	[Puppet] public Vector2 PuppetVelocity { get; set; }
	[Puppet] public int PuppetFaceDirection { get; set; }
	[Puppet] public String PuppetAnimation { get; set; }
	[Puppet] public bool PuppetIsDeadRevivable { get; set; }
	[Puppet] public bool PuppetIsDead { get; set; }	
	[Puppet] public Color PuppetColor { get; set; }
	[Puppet] public bool PuppetIsRevived { get; set; }
	[Puppet] public bool PuppetBeingRevived { get; set; }	
	[Puppet] public int PuppetReviveBarValue { get; set; }	

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

	[Export]
	private float knockBackSpeed;
	
	public bool Invincible = false;

	[Export]
	private int stunTime = 200;
	public int StunTime { get => stunTime; }
	private bool stunAfterHit = false;

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
	private float reviveTime = 2f;
	public float ReviveTime { get => reviveTime; }
	private bool isRevived = false;
	public bool IsRevived { 
		get => isRevived; 
		set {
			if (value) RevivePlayerPuppet();
			if (IsNetworkMaster()) isRevived = value;
		}    
	}
	public bool BeingRevived = false;
	private Area2D reviveDetect;
	public Vector2 SpawnPos;

	public GameManager gm;
	public int PlayerIndex;
	private Vector2 defaultSpriteScale;
	
	private CPUParticles2D walk;
	public CPUParticles2D Walk { get => walk; }
	private CPUParticles2D jump;
	public CPUParticles2D Jump { get => jump; }
	private CPUParticles2D slide;
	public CPUParticles2D Slide { get => slide; }
	
	private Vector2 screenSize;
	
	public Node2D NameTag;
	private Label NameTagLabel;
	public ProgressBar ReviveBar;
	
	// Skins
	public Texture Kanye = ResourceLoader.Load("res://Sprites/GoblinSkins/GoblinK.png") as Texture;
	public Texture Ukraine = ResourceLoader.Load("res://Sprites/GoblinSkins/GoblinU.png") as Texture;
	public Texture USA = ResourceLoader.Load("res://Sprites/GoblinSkins/GoblinUSA.png") as Texture;
	public int Id;
	
	public override void _Ready()
	{
		gm =  GetParent().GetNode<GameManager>("GameManager");
		PlayerIndex = gm.AddNewPlayer(this);
		gm.StaticPlayerList[PlayerIndex] = this;
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
		slide = GetNode<CPUParticles2D>("Particles/Slide");

		NameTag = GetNode<Node2D>("NameTag");
		NameTagLabel = NameTag.GetNode<Label>("Panel/Name");

		ReviveBar = GetNode<ProgressBar>("Revive/ProgressBar");

		defaultSpriteScale = sprite.Scale;
		FaceDirection = -1;

		normalGravity = Gravity;
		
		SpawnPos = Position;
		if (!Globals.SinglePlayer && GetTree().IsNetworkServer()) {
			gm.TeamSpawnLoc = Position;
		}
		
		// TODO: needs sync and work
		if (!Globals.SinglePlayer) { 
			if (GetTree().GetNetworkUniqueId() != 1) {
				var rand = new Random();
				var rNum = rand.Next(2, 5);
				if (rNum == 2) {
					GD.Print("knaye");
					sprite.SetTexture(Kanye);
				} else if (rNum == 3) {
					GD.Print("kraine");
					sprite.SetTexture(Ukraine);
				} else if (rNum == 4) {
					GD.Print("usa");
					sprite.SetTexture(USA);
				} 
			}
		}

		screenSize = GetViewport().GetVisibleRect().Size;

		State = new MoveState(this);
		if (!Globals.SinglePlayer) {
			NetworkNode = GetNode<Network>("/root/Network");
		} else {
			NameTag.Visible = false;
		}
	}

	public override void _Process(float delta)
	{
		if (Globals.SinglePlayer || IsNetworkMaster()) {
			State._Process(delta);
		}
		SynchronizeState();
		if (!Globals.SinglePlayer && !IsNetworkMaster())
			PuppetPosition = Position;
			
		if (gm.NumPlayers == 0 && !IsTeamResetting) {
			if (gm.TeamLives == 0) GameOver();
			else ReviveTeam();
		} 
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
							Color color, bool ir, bool br, 
							int rbv)
	{
		PuppetPosition = pos;
		PuppetVelocity = vel;
		PuppetFaceDirection = fd;
		PuppetAnimation = anim;
		PuppetColor = color;
		PuppetIsRevived = ir;
		PuppetBeingRevived = br;
		PuppetReviveBarValue = rbv;
	}
	
	[Remote]
	public void UpdateStateImportant(bool IsDeadRevivable, bool dead)
	{
		PuppetIsDeadRevivable = IsDeadRevivable;
		PuppetIsDead = dead;
		if (PuppetIsDeadRevivable && !IsDeadRevivable) 
			SetIsDeadRevivable();
		else if ( (!PuppetIsDeadRevivable && IsDeadRevivable) || (!PuppetIsDead && IsDead) ) 
			RevivePlayer();
		if (PuppetIsDead && !IsDead) 
			RemoveSelf();
		IsDeadRevivable = PuppetIsDeadRevivable;
		IsDead = PuppetIsDead;
	}
	
	public void BroadcastState() 
	{
		Rpc(nameof(UpdateStateImportant), IsDeadRevivable, IsDead);
		RpcUnreliable(nameof(UpdateState), Position, Velocity, FaceDirection, 
		animPlayer.CurrentAnimation, sprite.Modulate, isRevived, 
		BeingRevived, ReviveBar.Value);
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
		if (PuppetAnimation != null) 
			animPlayer.Play(PuppetAnimation);
		if (PuppetBeingRevived && !BeingRevived) {
			SetReviveBarVisible(true);
		} else if (!PuppetBeingRevived && BeingRevived) {
			SetReviveBarVisible(false);
		}
		BeingRevived = PuppetBeingRevived;
		ReviveBar.Value = PuppetReviveBarValue;
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
		if (!IsDead) TakeDamage(1);
	}
	
	public void ReviveTeam()
	{
		IsTeamResetting = true;
		gm.TeamReset();
	}
	
	public void SetIsDeadRevivable()
	{
		if (IsDeadRevivable || IsTeamResetting) return;
		GD.Print($"Set IsDeadRevivable {PlayerName}");
		GD.Print($"Removing {PlayerName} from {gm.LivePlayers} alive players");
		gm.LivePlayers -= 1;
		IsDeadRevivable = true;
		SetCollidable(false);
		SetColor(new Color( 1, 0, 0, 1 ));
	}
	
	public void SetCollidable(bool c)
	{
		SetCollisionLayerBit(1, c);
		SetCollisionLayerBit(7, !c);
		enemyHitBox.SetCollisionLayerBit(1, c);
		enemyHitBox.SetCollisionLayerBit(7, !c);
	}

	public void RemoveSelf() 
	{
		if (IsDead || IsTeamResetting) return;
		GD.Print($"Removing Self {PlayerName} in {State.GetType().Name}");
		if (!IsDeadRevivable) {
			GD.Print($"Removing {PlayerName} from {gm.LivePlayers} alive players");
			gm.LivePlayers -= 1; // Not already deducted
		}
		IsDead = true;
		Visible = false;
		Position = new Vector2(0, 0);
		if (!Globals.SinglePlayer && IsNetworkMaster()) 
			Rpc(nameof(UpdateStateImportant), IsDeadRevivable, IsDead);
		FreeCamera();
		if (gm.LivePlayers > 0) AttachCamera();
		gm.RemovePlayer(PlayerIndex);
	}

	public void Respawn()
	{
		if (IsNetworkMaster()) {
			Position = gm.TeamSpawnLoc;
			RevivePlayer();
			Cam cam = (Cam) GetParent().GetNode("Cam");
			cam.Player = this;
		}
		Visible = true;
		IsTeamResetting = false;
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
		if (IsDeadRevivable || IsDead || Invincible) return;
		if (Globals.SinglePlayer || IsNetworkMaster()) {
			base.TakeDamage(dmg);
			Lives -= 1;
			if (Lives <= 0) {
				GD.Print($"Player {PlayerName} entering deadstate");
				State = new DeadState(this);
			}
			else
				Position = SpawnPos;
		}
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
		GD.Print($"Gameover called by {PlayerName}");
		HandleDeathAnim();
		if (!Globals.SinglePlayer) {
			((Network) GetParent()).LeaveGame();
			QueueFree();
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

	public void TurnLeft() 
	{
		sprite.Position = new Vector2(1, 0);
		sprite.Scale = new Vector2(-defaultSpriteScale.x, defaultSpriteScale.y);
		FaceDirection = -1;
		wallDetect.Scale = Vector2.One;
		WallDetectFoot.Scale = Vector2.One;
		ladderDetectSide.Scale = new Vector2(-1, 1);
		walk.Position = new Vector2(3, 9);
		slide.Position = new Vector2(-5, 9);
	}

	public void TurnRight() 
	{
		sprite.Position = Vector2.Zero;
		sprite.Scale = defaultSpriteScale;
		FaceDirection = 1;
		wallDetect.Scale = new Vector2(-1, 1);
		WallDetectFoot.Scale = new Vector2(-1, 1);
		ladderDetectSide.Scale = Vector2.One;
		walk.Position = new Vector2(-3, 9);
		slide.Position = new Vector2(5, 9);
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
		if (!wallDetect.IsColliding()) {
			animPlayer.Play("Jump");
			slide.Emitting = false;
		}
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
	
	public int AttackEnemy() 
	{
		RandomNumberGenerator rand = new RandomNumberGenerator();
		// rand.Randomize();
		// float stunNotKillProbability = 0.1f;

		Godot.Collections.Array enemiesInRange = meleeArea.GetOverlappingBodies();
		int enemyCount = enemiesInRange.Count;
		// if (enemyCount > 1) {
		// 	stunNotKillProbability = 0.25f;
		// }

		foreach (Enemy enemy in enemiesInRange) {
			Vector2 enemyPosition = enemy.Position;
			// if (enemy.FaceDirection != FaceDirection) {
			// 	float temp = rand.Randf();
			// 	if (enemy.IsAttacking) {
			// 		enemy.KnockBack();
			// 		continue;
			// 	} 
			// }
			enemy.TakeDamage(meleeDamage, new Vector2(FaceDirection * 30f, 0));
		}
		return enemyCount;
	}
	
	public void KnockBack() 
	{
		Velocity.x = -1 * FaceDirection * knockBackSpeed;
		State.ExitState(new KnockBackState(this));
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
		}
		animPlayer.Play(name);
		((GoblinSound)GetNode("SoundEffects")).PlaySound(name);
	}
	
	public void SetName(String name)
	{
		PlayerName = name;
		NameTagLabel.Text = name;
	}
	
	public void RevivePlayer()
	{
		if (IsNetworkMaster()) {
			isRevived = true;
			SetColor(new Color(1, 1, 1, 1));
			SetReviveBarVisible(false);	
			State.ExitState(new MoveState(this));
		}
		GD.Print($"Reviving Player {PlayerName}");
		gm.LivePlayers += 1;
		SetCollidable(true);
		return;
	}
	
	public void RevivePlayerPuppet()
	{
		GD.Print($"REVIVE PUPPET {PlayerName}");
		RpcId(Int32.Parse(Name), nameof(RevivePlayerMaster));
	}
	
	[Remote]
	public void RevivePlayerMaster()
	{
		GD.Print($"REVIVE MASTER {PlayerName}");
		if (IsNetworkMaster()) RevivePlayer();
	}
	
	public void SetBeingRevivedPuppet(bool br)
	{
		RpcId(Int32.Parse(Name), nameof(SetBeingRevivedMaster), br);
	}
	
	[Remote]
	public void SetBeingRevivedMaster(bool br)
	{
		if (IsNetworkMaster()) {
			SetReviveBarVisible(br);
			BeingRevived = br;
		}
	}
	
	public void SetReviveBarVisible(bool visible)
	{
		NameTag.Visible = !visible;
		ReviveBar.Visible = visible;
	}
	
	public void SetId(int id)
	{
		Id = id;
	}
}
