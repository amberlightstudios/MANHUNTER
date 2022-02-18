using Godot;
using System;

namespace ShooterStates
{
	public class AttackState : ShooterState
	{
		private Goblin target;
		private float timer = 0f;

		public AttackState(StaticShooter shooter, Goblin target) 
		{   
			this.shooter = shooter;
			this.target = target;
			shooter.Velocity = Vector2.Zero;
			if (target.Position.x < shooter.Position.x) {
				shooter.TurnLeft();
			} else if (target.Position.x > shooter.Position.x) {
				shooter.TurnRight();
			}
			shooter.PlayAnimation("IdleAlert");
		}

		public override void _Process(float delta)
		{
			if (shooter.AnimPlayer.CurrentAnimation == "IdleAlert") {
				timer += delta;
			}

			if (timer > shooter.ShootFrequency) {
				shooter.PlayAnimation("Shoot");
				shooter.Shoot(target);
				timer = 0;
			}

			if (!shooter.AnimPlayer.IsPlaying() && shooter.DetectPlayer() == null) {
				ExitState(new NormalState(shooter));
				return;
			} else if (!shooter.AnimPlayer.IsPlaying()) {
				shooter.PlayAnimation("IdleAlert");
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			// Vector2 offset = shooter.PlayerOffset();
			// if (offset.Length() < shooter.EvadeDist) {
			//     ExitState(new EvadeState(shooter, offset));
			//     return;
			// }
		}
	}
}
