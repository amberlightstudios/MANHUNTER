using Godot;

public class Character : KinematicBody2D 
{
    [Export]
	protected int health;

    public void TakeDamage(int dmg) 
    {
        health -= dmg;
    }
}