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
				player.Slide.Emitting = false;
				ExitState(new JumpState(player, true));
			}

			if ((player.FaceDirection == 1 && Input.IsActionPressed("move_left"))
			|| (player.FaceDirection == -1 && Input.IsActionPressed("move_right"))) {
				if (player.OnGround()) {
					ExitState(new MoveState(player));
				} else {
					ExitState(new JumpState(player, true));
				}
			}

			if (!player.WallDetectFoot.IsColliding()) {
				player.Slide.Emitting = false;
			}

			if (!player.WallDetectFoot.IsColliding() && !player.CanWallClimb()) {
				ExitState(new JumpState(player, true));
			}

			isMovingUp = false;
			isMovingDown = false;
			if (Input.IsActionPressed("move_up")) {
				isMovingUp = true;
				player.PlayAnimation("Climb");
				player.Slide.Emitting = false;
			} else if (Input.IsActionPressed("move_down")) {
				isMovingDown = true;
			} 

			if (player.Velocity.y > 0) {
				player.PlayAnimation("Slide");
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
				player.Slide.Emitting = true;
			}
		}

		public override void ExitState(GoblinState newState)
		{
			player.Slide.Emitting = false;
			player.ReturnNormalGravity();
			base.ExitState(newState);
		}
	}
}
