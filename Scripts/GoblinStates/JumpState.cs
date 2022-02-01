using Godot;

namespace GoblinStates
{
	public class JumpState : GoblinState
	{
		public JumpState(Goblin player) 
		{
			this.player = player;
			player.Velocity.y = -10 * player.JumpSpeed;
		}

		public JumpState(Goblin player, bool isFallingDown) 
		{
			this.player = player;
			if (!isFallingDown)
				player.Velocity.y = -10 * player.JumpSpeed;
		}

		public override void _Process(float delta)
		{
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
			}

			if (Input.IsActionPressed("wall_climb") && Input.IsActionPressed("move_up")) {
				bool isWallClimbing = player.CanWallClimb();
				if (isWallClimbing) {
					ExitState(new WallClimbState(player));
				}
			}

			if (Input.IsActionJustPressed("Throw")) {
				ExitState(new ThrowState(player, new JumpState(player, true)));
			}

			// Play jump animation
			player.AnimPlayer.Play("Jump");
		}

		public override void _PhysicsProcess(float delta)
		{
			if (player.IsOnGround()) {
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
