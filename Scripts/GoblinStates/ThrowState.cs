using Godot;

namespace GoblinStates
{
	public class ThrowState : GoblinState
	{
		private GoblinState previousState;
		private float timer = 0;

		public ThrowState(Goblin player, GoblinState previousState) {
			this.player = player;
			this.previousState = previousState;
			player.Velocity = Vector2.Zero;
			player.SetZeroGravity();
			player.Throw();
		}

		public override void _Process(float delta)
		{
			timer += delta;

			if (timer > 0.6f) {
				ExitState(null);
				return;
			} 

			player.AnimPlayer.Play("Throw");
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
