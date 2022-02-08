using Godot;
using System;
using System.Threading.Tasks;

public class Enemy : Character
{
	protected Vector2 velocity = new Vector2(0, 0.1f);
	public Vector2 Velocity { get => velocity; set => velocity = value; }
	public bool IsGrabbed = false;
	public bool IsThrown = false;
	public bool IsThrownDown = false;

	protected bool isTakingDamage = false;

	protected AnimationPlayer animPlayer;
	public AnimationPlayer AnimPlayer { get => animPlayer; }

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
		TakeDamage(dmg);
		Position += knockbackDist;
	}

	public virtual void Death() 
	{
		GetParent().RemoveChild(this);
	}

	public virtual void PlayAnimation(string name) 
	{
		animPlayer.Play(name);
	}
}
