using Godot;

namespace ShooterStates
{
	public class EvadeState : ShooterState
	{
		public EvadeState(StaticShooter shooter, Vector2 offset) 
		{
			this.shooter = shooter;
			shooter.PlayAnimation("Walk");
			
			shooter.Velocity = new Vector2(shooter.EvadeSpeed * Mathf.Sign(offset.x), shooter.Velocity.y);
			if (shooter.Velocity.x < 0) {
				shooter.TurnRight();
			} else {
				shooter.TurnLeft();
			}
		}

		public override void _Process(float delta)
		{
			
		}

		public override void _PhysicsProcess(float delta)
		{
			if (shooter.PlayerOffset().Length() > shooter.EvadeDist * 2) {
				ExitState(new NormalState(shooter));
			}
		}
	}
}
