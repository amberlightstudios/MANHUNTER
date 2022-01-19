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
				player.AnimPlayer.Play("Walk");
			} 
			if (Input.IsActionPressed("move_right")) {
				player.Velocity.x = player.Speed;
				player.TurnRight();
				player.AnimPlayer.Play("Walk");
			}

			if (Input.IsActionJustPressed("jump")) {
				ExitState(new JumpState(player));
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
