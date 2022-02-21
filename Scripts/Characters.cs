using Godot;

public class Character : KinematicBody2D 
{
	[Export]
	protected int health;
	[Puppet]
	public int PuppetHealth { get; set; }
	[Export]
	public float Gravity { get; protected set; }
	protected float normalGravity;

	public virtual void TakeDamage(int dmg) 
	{
		health -= dmg;
	}
}
