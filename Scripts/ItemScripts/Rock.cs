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
	private Area2D groundDetect;

	public override void _Ready()
	{
		groundDetect = GetNode<Area2D>("GroundDetect");
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
		Godot.Collections.Array enemyHit = this.GetOverlappingBodies();
		foreach (Enemy e in enemyHit) {
			e.TakeDamage(damage, new Vector2(Direction * 10f, 0));
			GetParent().RemoveChild(this);
			return;
		}
		Godot.Collections.Array groundHit = groundDetect.GetOverlappingBodies();
		if (groundHit.Count > 0) {
			GetParent().RemoveChild(this);
			return;
		}
	}
}
