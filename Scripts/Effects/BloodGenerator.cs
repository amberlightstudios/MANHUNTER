using Godot;

namespace Effect 
{
	public class BloodGenerator : Node2D
	{
		PackedScene bloodScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Effects/Blood.tscn");
		public void GenerateBlood(int numParticles) 
		{
			for (int i = 0; i < numParticles; i++) {
				AddChild(bloodScene.Instance());
			}
		}
	}
}
