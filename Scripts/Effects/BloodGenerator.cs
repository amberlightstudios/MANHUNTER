using Godot;

namespace Effect 
{
	public class BloodGenerator : Node2D
	{
        private GameManager gm;

        public override void _Ready() 
        {
            gm = GetNode<GameManager>("/root/Main/GameManager");
        }

		PackedScene bloodScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Effects/Blood.tscn");
		public void GenerateBlood(int numParticles) 
		{
			for (int i = 0; i < numParticles; i++) {
                Node2D blood = bloodScene.Instance<Node2D>();
                blood.Position = GlobalPosition;
				gm.AddChild(blood);
			}
		}
	}
}
