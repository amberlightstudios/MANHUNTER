using Godot;

namespace GoblinStates
{
	public class ThrowState : GoblinState
	{
		private GoblinState previousState;
		private float timer = 0;
		private float throwAnimationSpeed;

		public ThrowState(Goblin player, GoblinState previousState, float throwAnimationSpeed) {
			this.player = player;
			this.previousState = previousState;
			this.throwAnimationSpeed = throwAnimationSpeed;
			player.Velocity = Vector2.Zero;
			player.SetZeroGravity();
			player.Throw(throwAnimationSpeed);
		}

		public override void _Process(float delta)
		{
			timer += delta;

			if (timer > 0.6f/throwAnimationSpeed) {
				ExitState(null);
				return;
			} 

			player.AnimPlayer.Play("Throw", customSpeed: throwAnimationSpeed);
		}

		public override void _PhysicsProcess(float delta)
		{
			
		}

		public override void ExitState(GoblinState newState)
		{
			player.EndThrow();
			player.ReturnNormalGravity();
			player.State = previousState;
		}
	}
}
