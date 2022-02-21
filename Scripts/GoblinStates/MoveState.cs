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
			|| (player.IsStandingOnLadder() && Input.IsActionPressed("move_down"))) {
				ExitState(new LadderClimbState(player));
				return;
			}
			
			if (Input.IsActionJustPressed("Dash")) {
				ExitState(new DashState(player));
				return;
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			if (player.IsStandingOnLadder()) {
				player.SetZeroGravity();
			} else {
				player.ReturnNormalGravity();
			}
		}

		public override void ExitState(GoblinState newState)
		{
			player.State = newState;
		}
	}
}
