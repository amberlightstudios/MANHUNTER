using Godot;
using System;
using System.Threading.Tasks;

public class Enemy : Character
{
	protected int touchDamage = 0;
	public int TouchDamage { get => touchDamage; }
	protected Vector2 velocity = new Vector2(0, 0.1f);
	public Vector2 Velocity { get => velocity; set => velocity = value; }
	public int FaceDirection;
	public int Fire = 0;
	public bool IsGrabbed = false;
	public bool IsThrown = false;
	public bool IsThrownDown = false;

	protected bool isTakingDamage = false;

	protected AnimationPlayer animPlayer;
	public AnimationPlayer AnimPlayer { get => animPlayer; }
	protected Sprite sprite;
	
	[Puppet] public int PuppetFire;
	[Puppet] public Vector2 PuppetVelocity;
	[Puppet] public string PuppetAnimation = "";
	[Puppet] public int PuppetFaceDirection;
	[Puppet] public Vector2 PuppetPosition;
		
	public override void _Ready()
	{
		
	}
	
	public override void _Process(float delta)
	{
		
	}   

	public override void _PhysicsProcess(float delta)
	{
		velocity.y += Gravity;
		MoveAndSlide(velocity);
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
		Rset(nameof(PuppetPosition), Position);		
		Rset(nameof(PuppetVelocity), Velocity);
		Rset(nameof(PuppetFaceDirection), FaceDirection);
		Rset(nameof(PuppetAnimation), animPlayer.CurrentAnimation);
		Rset(nameof(PuppetFire), Fire);
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
		isTakingDamage = true;
		if (health <= 0) {
			Death();
		}
		Task.Delay(1000).ContinueWith(t => isTakingDamage = false);
	}

	public virtual void TakeDamage(int dmg, Vector2 knockbackDist) 
	{
		if (GetTree().NetworkPeer != null) Rpc(nameof(TakeDamageRemote), dmg, knockbackDist);
		TakeDamage(dmg);
		Position += knockbackDist;
	}
	
	[Remote]
	public void TakeDamageRemote(int dmg, Vector2 knockbackDist)
	{
		TakeDamage(dmg, knockbackDist);
	}

	public virtual void Death() 
	{
		touchDamage = 0;
		GetParent().RemoveChild(this);
	}

	public virtual void PlayAnimation(string name) 
	{
		animPlayer.Play(name);
	}
	
	public virtual void Interpolate() {
		// TODO
	}
	
	public virtual void Shoot() {
		// Implemented in subclass
	}
}
