using Godot;

namespace GoblinStates
{
	public class DashState : GoblinState
	{
		public DashState(Goblin player) 
		{
			this.player = player;
			// player.IsInvincible = true;
			player.SetZeroGravity();
			player.Velocity = new Vector2(player.DashSpeed * player.FaceDirection, 0);
		}

		private float timer = 0f;
		public override void _Process(float delta)
		{
			timer += delta;
			if (timer > 0.2f) {
				if (player.OnGround()) {
					// if (timer > 0.28f) {
					ExitState(new MoveState(player));
					// } else {
					// 	player.IsInvincible = false;
					// 	player.Velocity = Vector2.Zero;
					// }
					return;
				}
				ExitState(new JumpState(player, true, true));
				return;
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			
		}

		public override void ExitState(GoblinState newState)
		{
			player.ReturnNormalGravity();
			player.IsInvincible = false;
			player.State = newState;
		}
	}
}
