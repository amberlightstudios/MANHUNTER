using Godot;

namespace ShooterStates
{
	public class DeathState : ShooterState
	{
		public DeathState(StaticShooter shooter) 
		{
			this.shooter = shooter;
			shooter.Velocity = new Vector2(0, shooter.Velocity.y);
			shooter.PlayAnimation("Death");
		}

		public override void _Process(float delta)
		{

		}

		public override void _PhysicsProcess(float delta)
		{
			
		}

		public override void ExitState(ShooterState newState)
		{
			return;
		}
	}
}
