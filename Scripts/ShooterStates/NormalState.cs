using Godot;
using System;

namespace ShooterStates
{
	public class NormalState : ShooterState
	{
		private float timer = 0f;
        private bool noticed = false;

		public NormalState(StaticShooter shooter) 
		{
			this.shooter = shooter;
			timer = 0f;
            noticed = false;
		}

        public NormalState(StaticShooter shooter, bool noticed) 
        {
            this.shooter = shooter;
            this.noticed = noticed;
        }

		private int tick = 0;
		public override void _Process(float delta)
		{
			tick++;
			if (tick > 20) {
				int random = new Random().Next(-1, 5);
				tick = 0;
				if (Math.Abs(random) == 1) {
					shooter.Velocity = new Vector2(shooter.RoamSpeed * random, shooter.Velocity.y);
					if (random < 0) {
						shooter.TurnLeft();
					} else {
						shooter.TurnRight();
					}
				} else {
					shooter.Velocity = new Vector2(0, shooter.Velocity.y);
				}
			}

            if (noticed) {
                timer += delta;
                if (timer > 2f) {
                    noticed = false;
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
			Goblin target = shooter.DetectPlayer();
			if (target != null) {
				if (!noticed) {
                    ExitState(new NoticeState(shooter, target));
                } else {
                    if (target.Position.x < shooter.Position.x) {
                        shooter.TurnLeft();
                    } else if (target.Position.x > shooter.Position.x) {
                        shooter.TurnRight();
                    }
                    ExitState(new AttackState(shooter));
                }
				return;
			} 

			shooter.EdgeDetect();
		}
	}
}
