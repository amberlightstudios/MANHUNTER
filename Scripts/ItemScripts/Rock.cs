using Godot;

public class Rock : Area2D
{
	[Export]
	private float speed = 10;
	[Export]
	private float lifespan = 3;    // in miliseconds
	[Export]
	private int damage = 1;

	public int Direction = -1;   // direction value can only be 1 or -1. 
	private float timer = 0f;

	public override void _Ready()
	{
		
	}

	public override void _Process(float delta) 
	{
		timer += delta;
		if (timer > lifespan) {
			GetParent().RemoveChild(this);
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		Position += new Vector2(Direction * speed, 0);
		
	}
}
