using Godot;

namespace EnemyScripts
{
	public class Trap : RigidBody2D
	{
		private Area2D playerDetect;
		private bool disable = false;

		public override void _Ready()
		{
			playerDetect = GetNode<Area2D>("PlayerDetect");
		}

		public override void _Process(float delta)
		{
			if (disable) {
				return;
			}

			Godot.Collections.Array players = playerDetect.GetOverlappingAreas();
			foreach (Area2D p in players) {
				Goblin player = p.GetParent<Goblin>();
				player.TakeDamage(5);
				disable = true;
			}
		}
	}
}
