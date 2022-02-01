using Godot;

namespace GoblinStates
{
	public class WallClimbState : GoblinState
	{
		bool isMovingUp = false;
		bool isMovingDown = false;

		public WallClimbState(Goblin player) 
		{
			this.player = player;
			player.SetZeroGravity();
		}

		public override void _Process(float delta)
		{
            if (!Input.IsActionPressed("wall_climb")) {
                ExitState(new JumpState(player, true));
            }

			if ((player.FaceDirection == 1 && Input.IsActionPressed("move_left"))
			|| (player.FaceDirection == -1 && Input.IsActionPressed("move_right"))) {
				if (player.IsOnGround()) {
					ExitState(new MoveState(player));
				} else {
					ExitState(new JumpState(player, true));
				}
			}

			if (!player.WallDetectFoot.IsColliding() && !player.CanWallClimb()) {
				ExitState(new JumpState(player, true));
			}

			isMovingUp = false;
			isMovingDown = false;
			if (Input.IsActionPressed("move_up")) {
				isMovingUp = true;
			} else if (Input.IsActionPressed("move_down")) {
				isMovingDown = true;
			} 
		}

		public override void _PhysicsProcess(float delta)
		{
			if (isMovingUp) {
				player.Velocity = new Vector2(0, -1 * player.WallClimbSpeed);
			} else if (isMovingDown) {
				player.Velocity = new Vector2(0, player.WallClimbSpeed);
			} else {
				player.Velocity = new Vector2(0, player.WallClimbSpeed * 0.2f);
			}
		}

		public override void ExitState(GoblinState newState)
		{
			player.ReturnNormalGravity();
			player.State = newState;
		}
	}
}
