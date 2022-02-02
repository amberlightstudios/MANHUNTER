using Godot;
using System;

namespace GoblinStates
{
	public class AttackState : GoblinState
	{
		private GoblinState previousState;
		private float timer = 0;
		private float animLength;
        private bool haveAttacked = false;
        private int previousFaceDirection;

		public AttackState(Goblin player, GoblinState previousState) 
		{
			this.player = player;
			player.Velocity = Vector2.Zero;
			player.AnimPlayer.Play("Melee1");
			animLength = player.AnimPlayer.CurrentAnimationLength;
            previousFaceDirection = player.FaceDirection;

			this.previousState = previousState;
		}

		public override void _Process(float delta)
		{
			timer += delta;

			if (timer > animLength) {
				ExitState(previousState);
				return;
			}

            player.Velocity.x = 0;
            if (Input.IsActionPressed("move_left")) {
				player.Velocity.x = -1 * player.Speed;
				player.TurnLeft();
            } 
            if (Input.IsActionPressed("move_right")) {
				player.Velocity.x = player.Speed;
				player.TurnRight();
            }
            if (player.FaceDirection != previousFaceDirection) {
                ExitState(previousState);
				return;
            }

            if (Input.IsActionJustPressed("Jump") && player.IsOnGround()) {
				JumpState newState = new JumpState(player);
				ExitState(newState);
                return;
			}

            if (timer > 0.2f && !haveAttacked) {
                player.AttackEnemy();
                haveAttacked = true;
            }

			player.AnimPlayer.Play("Melee1");
		}

		public override void _PhysicsProcess(float delta)
		{
			
		}

		public override void ExitState(GoblinState newState)
		{
			player.State = previousState;
		}
	}
}
