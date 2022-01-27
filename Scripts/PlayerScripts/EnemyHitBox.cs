using Godot;
using System;

namespace PlayerScripts 
{
	public class EnemyHitBox : Area2D
	{
		private Goblin player;

		public override void _Ready()
		{
			player = GetParent<Goblin>();
		}

		public override void _Process(float delta)
		{
			if (this.GetOverlappingBodies().Count > 0) {
				player.TakeDamage(1);
			}
		}
	}
}
