using Godot;
using System;
using System.Threading.Tasks;
using Effect;

public class Enemy : Character
{
	protected GameManager gm;
	public BloodGenerator BloodGenerator;
	protected Vector2 velocity = new Vector2(0, 0.1f);
	public Vector2 Velocity { get => velocity; set => velocity = value; }
	public int FaceDirection;
	public int Fire = 0;
	public int Melee = 0;
	public int GenerateBlood = 0;
	public bool Stunned = false;


	protected AnimationPlayer animPlayer;
	public AnimationPlayer AnimPlayer { get => animPlayer; }
	protected Sprite sprite;
	
	[Puppet] public int PuppetFire;
	[Puppet] public int PuppetMelee;	
	[Puppet] public Vector2 PuppetVelocity;
	[Puppet] public string PuppetAnimation = "";
	[Puppet] public int PuppetFaceDirection;
	[Puppet] public Vector2 PuppetPosition;
	[Puppet] public int PuppetGenerateBlood;
	
	public void InitEnemy()
	{
		gm =  GetParent().GetParent().GetNode<GameManager>("GameManager");
		gm.NumEnemies += 1;
		BloodGenerator = GetNode<BloodGenerator>("BloodGenerator");		
	}
	
	public override void _Ready()
	{
	}
	
	public override void _Process(float delta)
	{
	}   

	public override void _PhysicsProcess(float delta)
	{
	}
	
	public void SynchronizeState()
	{
		if (GetTree().NetworkPeer != null) {
			if  (GetTree().IsNetworkServer()) {
				BroadcastState();
			} else {
				Interpolate();
				ReceiveState();
			}
		}
	}
	
	public void BroadcastState()
	{
		RpcUnreliable(nameof(UpdateState), Position, Velocity, FaceDirection, 
					  animPlayer.CurrentAnimation, Fire, Melee, GenerateBlood);
	}
	
	[Remote]
	public void UpdateState(Vector2 pos, Vector2 vel, int fd, string anim,
							int fire, int melee, int blood)
	{
		PuppetPosition = pos;
		PuppetVelocity = vel;
		PuppetFaceDirection = fd;
		PuppetAnimation = anim;
		PuppetFire = fire;
		PuppetMelee = melee;
		PuppetGenerateBlood = blood;
	}
	
	public void ReceiveState()
	{
		Velocity = PuppetVelocity;
		Position = PuppetPosition;
		if (FaceDirection != PuppetFaceDirection) {
			FaceDirection = PuppetFaceDirection;
			if (PuppetFaceDirection == -1) TurnLeft();
			else TurnRight();
		}
		if (PuppetAnimation != animPlayer.CurrentAnimation && PuppetAnimation != "") {
			animPlayer.Play(PuppetAnimation);
		}
		if (PuppetFire > Fire) {
			Fire += 1;
			Shoot();
		}
		if (PuppetMelee > Melee) {
			Melee += 1;
			Attack();
		}
		if (PuppetGenerateBlood > GenerateBlood) {
			BloodGenerator.GenerateBlood(PuppetGenerateBlood);
		}
		GenerateBlood = PuppetGenerateBlood;
	}

	public virtual void TurnLeft() 
	{
		FaceDirection = -1;
		sprite.Position = new Vector2(1, 0);
		sprite.Scale = new Vector2(-1, 1);
	}

	public virtual void TurnRight() 
	{
		FaceDirection = 1;
		sprite.Position = Vector2.Zero;
		sprite.Scale = Vector2.One;
	}

	public void UpdatePosition(Vector2 pos, Vector2 scaleMultiplier) 
	{
		Position = pos;
		Scale *= scaleMultiplier;
	}

	public override void TakeDamage(int dmg)
	{
		base.TakeDamage(dmg);
		if (health <= 0 || Stunned) {
			Death();
		} else {
			Stun();
		}
	}

	public virtual void TakeDamage(int dmg, Vector2 knockbackDist) 
	{
		TakeDamage(dmg);
		Position += knockbackDist;
	}
	
	public virtual void KnockBack() {}
	
	public virtual void Death() 
	{
		QueueFree();
	}

	public virtual void Stun() {}

	public virtual void PlayAnimation(string name) 
	{
		if (name == null) {
			animPlayer.Stop();
		}
		animPlayer.Play(name);
	}
	
	public virtual void Interpolate() {
		velocity.y += Gravity;
		MoveAndSlide(velocity);
	}
	
	public virtual void Shoot() {
		// Implemented in subclass
	}
	
	public virtual int Attack() {
		// Implemented in subclass		
		return 0;
	}
}
