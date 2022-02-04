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
				player.PlayAnimation("Walk");
			} 
			if (Input.IsActionPressed("move_right")) {
				player.Velocity.x = player.Speed;
				player.TurnRight();
				player.PlayAnimation("Walk");
			}

			if (Input.IsActionJustPressed("Jump") && player.IsOnGround()) {
				ExitState(new JumpState(player));
			} else if (!player.IsOnGround()) {
				ExitState(new JumpState(player, true));
			}

			if (Input.IsActionJustPressed("Throw") && player.RocksCount > 0) {
				ExitState(new ThrowState(player, this));
			}

			if (Input.IsActionJustPressed("Attack")) {
				ExitState(new AttackState(player, this));
			}

			if (player.Velocity.Length() == 0) 
				player.PlayAnimation("Idle");
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
