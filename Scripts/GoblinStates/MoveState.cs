using Godot;

namespace GoblinStates {
	public class MoveState : GoblinState 
	{
		public MoveState(Goblin player) {
			this.player = player;
		}

		public override void _Process(float delta)
		{
			player.Velocity.x = 0;
			if (Input.IsActionPressed("move_left")) {
				player.Velocity.x = -1 * player.Speed;
				player.TurnLeft();
				player.AnimPlayer.Play("walk");
			} 
			if (Input.IsActionPressed("move_right")) {
				player.Velocity.x = player.Speed;
				player.TurnRight();
				player.AnimPlayer.Play("walk");
			}

			if (Input.IsActionJustPressed("jump") && player.IsOnGround()) {
				JumpState newState = new JumpState(player);
				ExitState(newState);
			}

			if (Input.IsActionJustPressed("Throw")) {
				ExitState(new ThrowState(player, this, 1.2f));
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

			if (player.Velocity.Length() == 0) 
				player.AnimPlayer.Play("Idle");
		}

		public override void _PhysicsProcess(float delta)
		{

		}

		public override void ExitState(GoblinState newState)
		{
			player.State = newState;
		}
	}
}
