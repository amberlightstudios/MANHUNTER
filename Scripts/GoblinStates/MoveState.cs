using Godot;

namespace GoblinStates {
	public class MoveState : GoblinState 
	{
		public bool IsHoldingEnemy = false;
		private Enemy heldEnemy;

		public MoveState(Goblin player) {
			this.player = player;
		}

		public MoveState(Goblin player, Enemy enemy) {
			this.player = player;
			IsHoldingEnemy = true;
			heldEnemy = enemy;
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
				JumpState newState = new JumpState(player);
				newState.IsHoldingEnemy = IsHoldingEnemy;
				ExitState(newState);
			}

			if (Input.IsActionJustPressed("Grab")) {
				if (!IsHoldingEnemy) {
					ExitState(new ThrowState(player));
				} else {
					player.ThrowEnemy(heldEnemy);
					IsHoldingEnemy = false;
				}
			} 

			if (player.Velocity.Length() == 0) 
				player.AnimPlayer.Play("Idle");
		}

		public override void _PhysicsProcess(float delta)
		{
			if (IsHoldingEnemy) {
				heldEnemy.UpdatePosition(player.ThrowPoint, 
										player.FaceDirection == 1 ? Vector2.One : new Vector2(-1, 1));
			}
		}

		public override void ExitState(GoblinState newState)
		{
			player.State = newState;
		}
	}
}
