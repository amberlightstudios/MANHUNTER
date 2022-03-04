using Godot;

namespace ShooterStates
{
	public class DeathState : ShooterState
	{
		bool isPlayingAnim = false;

		public DeathState(StaticShooter shooter) 
		{
			shooter.GetParent().RemoveChild(shooter); // Remove this later
			this.shooter = shooter;
			shooter.Velocity = new Vector2(0, shooter.Velocity.y);
		}

		public override void _Process(float delta)
		{
			if (shooter.OnGround() && !isPlayingAnim) {
				// shooter.PlayAnimation("Death");
				isPlayingAnim = true;
			}

			if (isPlayingAnim && !shooter.AnimPlayer.IsPlaying()) {
				shooter.GetParent().RemoveChild(shooter);
			}
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
