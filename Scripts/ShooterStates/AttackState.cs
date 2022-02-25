using Godot;
using System;

namespace ShooterStates
{
	public class AttackState : ShooterState
	{
		private float timer = 0f;

		public AttackState(StaticShooter shooter, float timer = 0) 
		{   
			this.shooter = shooter;
			shooter.Velocity = Vector2.Zero;
			shooter.PlayAnimation("IdleAlert");
			this.timer = timer;
		}

		public override void _Process(float delta)
		{
			if (!shooter.AnimPlayer.IsPlaying() && shooter.DetectPlayer() == null) {
				ExitState(new NormalState(shooter));
				return;
			} else if (!shooter.AnimPlayer.IsPlaying()) {
				Goblin target = shooter.DetectPlayer();
				if (target.Position.x < shooter.Position.x) {
					shooter.TurnLeft();
				} else if (target.Position.x > shooter.Position.x) {
					shooter.TurnRight();
				}
				shooter.PlayAnimation("IdleAlert");
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			if (shooter.AnimPlayer.CurrentAnimation == "IdleAlert") {
				timer += delta;
			}

			if (timer > shooter.ShootFrequency) {
				shooter.PlayAnimation("Shoot");
				shooter.Fire += 1;				
				shooter.Shoot();
				timer = 0;
			}
		}
	}
}
