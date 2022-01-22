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

            if (Input.IsActionJustPressed("Grab") && IsHoldingEnemy) {
                player.ThrowEnemy(HeldEnemy);
				IsHoldingEnemy = false;
                HeldEnemy = null;
            }

            // Play jump animation
            player.AnimPlayer.Play("jump");
        }

        public override void _PhysicsProcess(float delta)
        {
            if (player.IsOnGround()) {
                MoveState newState = new MoveState(player);
                newState.IsHoldingEnemy = IsHoldingEnemy;
                newState.HeldEnemy = HeldEnemy;
                ExitState(newState);
                return;
            }

            if (IsHoldingEnemy) {
				HeldEnemy.UpdatePosition(player.ThrowPoint, 
										player.FaceDirection == 1 ? Vector2.One : new Vector2(-1, 1));
			}
        }

        public override void ExitState(GoblinState newState)
        {
            player.State = newState;
        }
    }
}