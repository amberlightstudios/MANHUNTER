using Godot;
using System;

namespace GoblinStates
{
	public class DeadState : GoblinState 
	{
		private float timer = 0;
		private float ghostTimeBeforeDeath = 1f;

		public DeadState(Goblin player) 
		{
			this.player = player;
			player.Velocity = Vector2.Zero;
			player.ReturnNormalGravity();
			player.SetIsKilled();				
		}

		public override void _Process(float delta)
		{
			timer += delta;

			if (!player.AnimPlayer.IsPlaying()) {
				player.PlayAnimation("Ghost");
				timer = 0;
			}

			if (Globals.SinglePlayer && timer > ghostTimeBeforeDeath) {
				player.GameOver();
			}
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
