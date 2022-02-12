using Godot;

namespace BabyStates
{
	public class JumpState : BabyState 
	{
		public JumpState(Baby baby) 
		{
			this.baby = baby;
			baby.Velocity = new Vector2(baby.GetAttackDist() * 1.2f, -10 * baby.JumpSpeed);
		}

		public override void _Process(float delta)
		{
			baby.PlayAnimation("Jump");
		}

		public override void _PhysicsProcess(float delta)
		{
			if (baby.OnGround() && baby.Velocity.y >= 0) {
				ExitState(new MoveState(baby));
			}   
			
			if (baby.TopDetect.IsColliding() && baby.Velocity.y < 0) {
				baby.Velocity = new Vector2(baby.Velocity.x, 0);
			}

			if (baby.Velocity.x < 0) {
				baby.TurnLeft();
			} else if (baby.Velocity.x > 0) {
				baby.TurnRight();
			}
		}
	}
}
