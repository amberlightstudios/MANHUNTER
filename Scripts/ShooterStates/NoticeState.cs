using Godot;

namespace ShooterStates
{
	public class NoticeState : ShooterState
	{
		private float timer = 0f;
		private Goblin target;

		public NoticeState(StaticShooter shooter, Goblin target) 
		{
			this.shooter = shooter;
			this.target = target;
			
			shooter.Velocity = Vector2.Zero;
			if (target.Position.x < shooter.Position.x) {
				shooter.TurnLeft();
			} else if (target.Position.x > shooter.Position.x) {
				shooter.TurnRight();
			}
		}

		public override void _Process(float delta)
		{
			timer += delta;
			if (timer > shooter.NoticeTime) {
				ExitState(new AttackState(shooter));
			} 
		}

		public override void _PhysicsProcess(float delta)
		{
			
		}
	}
}
