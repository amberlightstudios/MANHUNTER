using Godot;
using System;

namespace GoblinStates
{
	public class DeadState : GoblinState 
	{
		private float timer = 0, animTimer = 0;
		private float ghostTimeBeforeDeath = 2.8f;
		private bool hasSetDead = false;
		
		public DeadState(Goblin player) 
		{
			this.player = player;
			player.Velocity = Vector2.Zero;
			player.Invincible = true;
			player.ReturnNormalGravity();
			player.PlayAnimation("Death");
			hasSetDead = false;
		}

		public override void _Process(float delta)
		{
			if (!hasSetDead && !Globals.SinglePlayer) {
				if (player.gm.LivePlayers > 1)
					player.SetIsDeadRevivable(); // can revive
				hasSetDead =true;
			}	
			animTimer += delta;

			if (!player.AnimPlayer.IsPlaying()) {
				player.PlayAnimation("Ghost");
				animTimer = 0;
			}

			if (Globals.SinglePlayer && animTimer > ghostTimeBeforeDeath) {
				player.GameOver();
			} else if (animTimer > ghostTimeBeforeDeath) {
				if (player.gm.LivePlayers == 0 || player.DropDead) {
					player.RemoveSelf(); // cannot revive
				}
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
			player.Invincible = false;
			GD.Print($"Starting to Exiting to MoveState for ${player.PlayerName}");			
			if (player.IsRevived) {
				player.State = new MoveState(this.player);
				player.IsDeadRevivable = false;
				player.IsDead = false;
				player.IsRevived = false;
				player.DropDead = false;
				player.BeingRevived = false;			
				player.ReviveBar.Value = 0;	
			}
			return;
		}
	}
}
