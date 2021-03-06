using Godot;
using System;

namespace GoblinStates
{
	public class DeadState : GoblinState 
	{
		private float timer = 0;

		public DeadState(Goblin player) 
		{
			this.player = player;
			player.Velocity = Vector2.Zero;
			player.ReturnNormalGravity();

			if (!Globals.SinglePlayer) {
				player.SetIsKilled();				
			} else {
				player.RemoveSelf();
			}
		}

		public override void _Process(float delta)
		{
			
		}

		public override void _PhysicsProcess(float delta)
		{
			player.Velocity.x = 0;

			if (!player.BeingRevived) {
				timer += delta;
				if (timer > 12f) {
					player.RemoveSelf();
				} 
			} else {
				if ((Math.Round(timer * 100, 0) * 100) % 50 == 0)  {
					player.ReviveBar.Value += 1;
				}
			}
		}

		public override void ExitState(GoblinState newState)
		{
			if (player.IsRevived) {
				player.State = new MoveState(this.player);
				player.Killed = false;
				player.IsRevived = false;
				player.BeingRevived = false;			
				player.ReviveBar.Value = 0;	
			}
			return;
		}
	}
}
