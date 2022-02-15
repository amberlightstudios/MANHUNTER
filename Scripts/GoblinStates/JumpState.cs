using Godot;

namespace GoblinStates
{
	public class JumpState : GoblinState
	{
		private int tick = 0;
		private bool highJump = false;
		private bool haveDashed = false;

		public JumpState(Goblin player) 
		{
			this.player = player;
			player.Velocity.y = -8 * player.JumpSpeed;

			// Play jump animation
			player.PlayAnimation("Jump");
		}

		public JumpState(Goblin player, bool isFallingDown) 
		{
			this.player = player;
			if (!isFallingDown)
				player.Velocity.y = -6 * player.JumpSpeed;
		}

		public JumpState(Goblin player, bool isFallingDown, bool haveDashed) 
		{
			this.player = player;
			this.haveDashed = haveDashed;
			if (!isFallingDown)
				player.Velocity.y = -6 * player.JumpSpeed;
		}

		public override void _Process(float delta)
		{
			if (Input.IsActionPressed("Jump") && !highJump) {
				tick += 1;
				if (tick > 12) {
					player.Velocity.y += -3f * player.JumpSpeed;
					highJump = true;
				}
			}

			if (!Input.IsActionPressed("move_left") && !Input.IsActionPressed("move_right")) {
				player.Velocity.x = 0;
			}

			if (Input.IsActionPressed("move_left")) {
				player.Velocity.x = -1 * player.Speed;
				player.TurnLeft();
			}

			if (Input.IsActionPressed("move_right")) {
				player.Velocity.x = player.Speed;
				player.TurnRight();
			}

			if (Input.IsActionJustPressed("Attack")) {
				ExitState(new AttackState(player, this));
				return;
			}

			if (Input.IsActionPressed("wall_climb")) {
				bool isWallClimbing = player.CanWallClimb();
				if (isWallClimbing) {
					ExitState(new WallClimbState(player));
					return;
				}
			}

			if (Input.IsActionJustPressed("Throw") && player.RocksCount > 0) {
				ExitState(new ThrowState(player, new JumpState(player, true)));
				return;
			}

			if (player.OnLadder() && (Input.IsActionPressed("move_up") || Input.IsActionPressed("move_down"))) {
				ExitState(new LadderClimbState(player));
				return;
			}

			if (Input.IsActionJustPressed("Dash") && !haveDashed) {
				ExitState(new DashState(player));
				return;
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			if (player.OnGround()) {
				MoveState newState = new MoveState(player);
				ExitState(newState);
				return;
			}
		}

		public override void ExitState(GoblinState newState)
		{
			player.State = newState;
		}
	}
}
