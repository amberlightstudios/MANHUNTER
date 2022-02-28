using Godot;

namespace GoblinStates
{
	public class DashState : GoblinState
	{
		public DashState(Goblin player) 
		{
			this.player = player;
			player.Invincible = true;
            if (!player.OnGround()) {
                player.SetZeroGravity();
			    player.Velocity = new Vector2(player.DashSpeed * player.FaceDirection, 0);
            } else {
                player.Velocity = new Vector2(player.DashSpeed * 0.7f * player.FaceDirection, 0);
            }
            player.PlayAnimation("Idle");
		}

		private float timer = 0f;
		public override void _Process(float delta)
		{
			timer += delta;
            if (timer > 0.1f) {
                player.Invincible = false;
            }
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
			player.Invincible = false;
			player.State = newState;
		}
	}
}
