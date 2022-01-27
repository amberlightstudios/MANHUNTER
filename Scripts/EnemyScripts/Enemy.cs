using Godot;
using System;

public class Enemy : Character
{
	protected Vector2 velocity = new Vector2(0, 0.1f);
	public Vector2 Velocity { get => velocity; set => velocity = value; }
	
	public bool IsGrabbed = false;
	public bool IsThrown = false;
	public bool IsThrownDown = false;

	public override void _Ready()
	{
		
	}

	public override void _Process(float delta)
	{
		
	}

	public override void _PhysicsProcess(float delta)
	{

	}

	public void UpdatePosition(Vector2 pos, Vector2 scaleMultiplier) 
	{
		Position = pos;
		Scale *= scaleMultiplier;
	}

	public override void TakeDamage(int dmg)
	{
		base.TakeDamage(dmg);
		if (health <= 0) {
			GD.Print("Im dead");
		}
	}
}
