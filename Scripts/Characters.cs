using Godot;

public class Character : KinematicBody2D 
{
    [Export]
	protected int health;
    [Export]
	public float Gravity { get; protected set; }

    public virtual void TakeDamage(int dmg) 
    {
        health -= dmg;
    }
}