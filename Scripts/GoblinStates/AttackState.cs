using Godot;
using System;

namespace GoblinStates
{
	public class AttackState : GoblinState
	{
		private GoblinState previousState;
		private float timer = 0;
		private bool haveAttacked = false;
		private int previousFaceDirection;
		private float speed;
        private string[] idleAnimationNames = new string[] { "BasicIdle", "BasicIdle2" };
        private string[] movingAnimationNames = new string[] { "BasicMoving", "BasicMoving2" };

		public AttackState(Goblin player, GoblinState previousState) 
		{
			this.player = player;
			
			previousFaceDirection = player.FaceDirection;
			speed = player.Speed;

			this.previousState = previousState;

            int randomAnim = new Random().Next(0, 2);
            if (player.Velocity.Length() > 0) {
                player.PlayAnimation(movingAnimationNames[randomAnim]);
            } else {
                player.PlayAnimation(idleAnimationNames[randomAnim]);
            }
		}

		public override void _Process(float delta)
		{
			timer += delta;

			if (!player.AnimPlayer.IsPlaying()) {
				ExitState(previousState);
				return;
			}

            player.Velocity.x = 0;
            if (Input.IsActionPressed("move_left")) {
                player.Velocity.x = -1 * speed;
                player.TurnLeft();
            } 
            if (Input.IsActionPressed("move_right")) {
                player.Velocity.x = speed;
                player.TurnRight();
            }
            if (player.FaceDirection != previousFaceDirection) {
                ExitState(previousState);
                return;
            }

            if (Input.IsActionJustPressed("Jump") && player.OnGround()) {
                ExitState(new JumpState(player));
                return;
            }

            if (!haveAttacked) {
                haveAttacked = player.AttackEnemy();
            }
		}

		public override void _PhysicsProcess(float delta)
		{
			speed -= player.AttakDeceleration;
		}

		public override void ExitState(GoblinState newState)
		{
            // player.AnimPlayer.Stop();
			player.State = newState;
		}
	}
}
