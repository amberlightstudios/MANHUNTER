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
        private bool soundPlayed = false;
        private int count = 0;

		public AttackState(Goblin player, GoblinState previousState) 
		{
			this.player = player;
			player.IsAttacking = true;
			
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
			if (Input.IsActionJustReleased("speed_boost")) {
				SpeedBoost = 1;
			}

			if (!player.AnimPlayer.IsPlaying()) {
				if (!player.OnGround()) {
					ExitState(new JumpState(player, true));
					player.PlayAnimation("Jump");
					return;
				}
				ExitState(previousState);
				return;
			}

			player.Velocity.x = 0;
			if (Input.IsActionPressed("move_left")) {
				if (player.FaceDirection == 1) {
					SpeedBoost = 1;
				}
				player.Velocity.x = -1 * speed * SpeedBoost;
				player.TurnLeft();
			} 
			if (Input.IsActionPressed("move_right")) {
				if (player.FaceDirection == -1) {
					SpeedBoost = 1;
				}
				player.Velocity.x = speed * SpeedBoost;
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
				int status = player.AttackEnemy();
				if (status > 0) {
					haveAttacked = true;
                    if (!soundPlayed) {
                        player.PlaySound("AttackHit");
                        soundPlayed = true;
                    }
				} else if (status == 0) {
                    if (!soundPlayed && count > 3) {
                        player.PlaySound("Attack");
                        soundPlayed = true; 
                    } 
                } else if (status == -1) {
					return;
				} 
                
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			timer += delta;

			if (timer >= 0.03f && timer <= 0.15f) {
				player.DeflectBullet();
			}

			if (player.IsRunningIntoLadder()) {
				player.SetLadderCollision(false);
			} else {
				player.SetLadderCollision(true);
			}

			speed -= player.AttakDeceleration;
		}

		public override void ExitState(GoblinState newState) 
		{
			player.IsAttacking = false;
			base.ExitState(newState);    
		}
	}
}
