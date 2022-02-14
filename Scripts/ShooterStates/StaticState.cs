using Godot;
using System;

namespace ShooterStates
{
	public class StaticState : ShooterState
	{
		private float timer = 0f;

		public StaticState(StaticShooter shooter) 
		{
			this.shooter = shooter;
			timer = 0f;
		}

		public StaticState(StaticShooter shooter, float timer)
		{
			this.shooter = shooter;
			this.timer = timer;
		}

		public override void _Process(float delta)
		{
			shooter.PlayAnimation("Idle");
		}

		public override void _PhysicsProcess(float delta)
		{
			Goblin target = shooter.DetectPlayer();
			if (target != null) {
				if (target.Position.x < shooter.Position.x) {
					shooter.TurnLeft();
				} else if (target.Position.x > shooter.Position.x) {
					shooter.TurnRight();
				}

				timer += delta;
				if (timer > shooter.ShootFrequency) {
					ExitState(new AttackState(shooter, target));
					return;
				}
			} else {
				timer = 0;
			}
		}
	}
}
