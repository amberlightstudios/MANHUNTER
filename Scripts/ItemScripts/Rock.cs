using Godot;
using System.Threading.Tasks;

public class Rock : Area2D
{
	[Export]
	private float speed;
	[Export]
	private int lifespan = 5000;    // in miliseconds

	public int Direction = -1;   // direction value can only be 1 or -1. 

	public override void _Ready()
	{
		
	}

	public override void _PhysicsProcess(float delta)
	{
		Position += new Vector2(Direction * speed, 0);
		
		Task.Delay(lifespan).ContinueWith(t => {
			GetParent().RemoveChild(this);
		});
	}
}
