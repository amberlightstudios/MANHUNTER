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

			if (Input.IsActionJustPressed("jump") && player.IsOnGround()) {
				JumpState newState = new JumpState(player);
				// newState.IsHoldingEnemy = IsHoldingEnemy;
				// newState.HeldEnemy = HeldEnemy;
				newState.Bomb = Bomb;
				newState.ThrowForceMultiplier = ThrowForceMultiplier;
				ExitState(newState);
			}

			if (Input.IsActionJustPressed("Grab")) {
				// if (!IsHoldingEnemy) {
				// 	ExitState(new ThrowState(player));
				// } else {
				// 	if (Input.IsActionPressed("move_down")) {
				// 		player.ThrowDownEnemy(HeldEnemy);
				// 	} else {
				// 		player.ThrowEnemy(HeldEnemy);
				// 	}
				// 	IsHoldingEnemy = false;
				// 	HeldEnemy = null;
				// }
				Bomb = player.CreateBomb("Bomb");
				ThrowForceMultiplier = player.LowThrowMultiplier;
			} else if (Input.IsActionJustReleased("Grab")) {
				player.ThrowBomb();
			} else if (Input.IsActionPressed("Grab")) {
				ThrowForceMultiplier += player.AdditionalThrowMultiplier;
				if (ThrowForceMultiplier >= 1) {
					ThrowForceMultiplier = 1f;
					player.ThrowBomb();
				}
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
			if (IsHoldingEnemy) {
				HeldEnemy.UpdatePosition(player.ThrowPoint, 
										player.FaceDirection == 1 ? Vector2.One : new Vector2(-1, 1));
			}

			if (Bomb != null) {
				Bomb.Position = player.ThrowPoint;
			}
		}

		public override void ExitState(GoblinState newState)
		{
			player.State = newState;
		}
	}
}
