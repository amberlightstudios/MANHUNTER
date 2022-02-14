using Godot;
using System;

namespace ShooterStates
{
	public class NormalState : ShooterState
	{
		private float timer = 0f;

		public NormalState(StaticShooter shooter) 
		{
			this.shooter = shooter;
			timer = 0f;
		}

		public NormalState(StaticShooter shooter, float timer)
		{
			this.shooter = shooter;
			this.timer = timer;
		}

        private int tick = 0;
		public override void _Process(float delta)
		{
            tick++;
            if (tick > 40) {
                int random = new Random().Next(-1, 3);
                tick = 0;
                if (Math.Abs(random) == 1) {
                    shooter.Velocity = new Vector2(shooter.RoamSpeed * random, shooter.Velocity.y);
                    if (random < 0) {
                        shooter.TurnLeft();
                    } else {
                        shooter.TurnRight();
                    }
                } 
            }
            
            if (shooter.Velocity.x != 0) {
                shooter.PlayAnimation("Walk");
            } else {
                shooter.PlayAnimation("Idle");
            }
		}

		public override void _PhysicsProcess(float delta)
		{
			Vector2 offset = shooter.PlayerOffset();
			if (offset.Length() < shooter.EvadeDist) {
				ExitState(new EvadeState(shooter, offset));
				return;
			}

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
				if (timer > 0) {
					timer -= delta/10;
				} else {
					timer = 0;
				}
			}

            shooter.EdgeDetect();
		}
	}
}
