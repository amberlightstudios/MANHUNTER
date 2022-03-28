using Godot;

namespace Effect 
{
	public class BloodGenerator : Node2D
	{
		PackedScene bloodScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Effects/Blood.tscn");

		public override void _Ready() 
		{
		}

		public void GenerateBlood(int numParticles) 
		{
			for (int i = 0; i < numParticles; i++) {
				Node2D blood = bloodScene.Instance<Node2D>();
				blood.Position = ToLocal(GlobalPosition);
				AddChild(blood);
			}
		}
	}
}
