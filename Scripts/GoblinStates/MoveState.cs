using Godot;

namespace GoblinStates {
	public class MoveState : GoblinState 
	{
		private float timer = 0f;

		public MoveState(Goblin player, float timer = 0) {
			this.player = player;
			this.timer = timer;
		}

		public override void _Process(float delta)
		{
			timer += delta;

			// if (timer < 0.2f 
			// && ((player.FaceDirection == 1 && Input.IsActionJustPressed("move_right")) 
			// || (player.FaceDirection == -1 && Input.IsActionJustPressed("move_left")))
			// || Input.IsActionPressed("speed_boost")) {
			// 	SpeedBoost = player.SpeedBoost;
			// }

			// if (Input.IsActionJustReleased("speed_boost") 
			// || (Input.IsActionJustReleased("move_left") || Input.IsActionJustReleased("move_right"))) {
			// 	SpeedBoost = 1;
			// }

			if (Input.IsActionJustPressed("move_right") || Input.IsActionJustPressed("move_left")) {
				timer = 0;
			}

			player.Velocity.x = 0;
			if (Input.IsActionPressed("move_left")) {
				player.Velocity.x = -1 * player.Speed * SpeedBoost;
				player.TurnLeft();
				player.PlayAnimation("Walk");

			} 
			if (Input.IsActionPressed("move_right")) {
				player.Velocity.x = player.Speed * SpeedBoost;
				player.TurnRight();
				player.PlayAnimation("Walk");
			}
			
			if (player.Velocity.x == 0) 
				player.PlayAnimation("Idle");
				
			if (player.Velocity.x == 0)
				player.Walk.Emitting = false;

			if (Input.IsActionJustPressed("Jump") && player.OnGround()) {
				ExitState(new JumpState(player));
				return;
			} else if (!player.OnGround()) {
				ExitState(new JumpState(player, true));
				return;
			}

			if (Input.IsActionJustPressed("Throw") && player.RocksCount > 0) {
				ExitState(new ThrowState(player, this));
				return;
			}

			if (Input.IsActionJustPressed("Attack")) {
				ExitState(new AttackState(player, this));
				return;
			}

			if ((player.OnLadder() && Input.IsActionPressed("move_up")) 
			|| (player.OnGround() && Input.IsActionPressed("move_down"))) {
				ExitState(new LadderClimbState(player));
				return;
			}
			
			if (!Globals.SinglePlayer && Input.IsActionJustPressed("Revive")) {
				Goblin target = player.FindReviveTarget();
				if (target != null) {
					ExitState(new ReviveState(player, target));
					return;
				}
			}

			// if (Input.IsActionJustPressed("Dash")) {
			// 	ExitState(new DashState(player));
			// 	return;
			// }
		}

		public override void _PhysicsProcess(float delta)
		{
			if (player.IsRunningIntoLadder()) {
				player.SetLadderCollision(false);
			} else {
				player.SetLadderCollision(true);
			}
		}
	}
}
