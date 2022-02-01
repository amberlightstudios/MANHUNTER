using Godot;

namespace GoblinStates
{
	public class ThrowState : GoblinState
	{
		private GoblinState previousState;
		private float timer = 0;
		private float animLength;
		private bool rockGenerated = false;

		public ThrowState(Goblin player, GoblinState previousState) {
			this.player = player;
			this.previousState = previousState;
			player.Velocity = Vector2.Zero;
			// player.SetZeroGravity();
			player.Throw();
			animLength = player.AnimPlayer.CurrentAnimationLength;
		}

		public override void _Process(float delta)
		{
			timer += delta;

			if (timer > animLength) {
				ExitState(null);
				return;
			} 

			if (Input.IsActionJustPressed("Jump") && player.IsOnGround()) {
				JumpState newState = new JumpState(player);
				ExitState(newState);
				return;
			}

			if (timer > animLength - 0.15f && !rockGenerated) {
				player.GenerateRock();
				rockGenerated = true;
			}

			player.AnimPlayer.Play("Throw");
		}

		public override void _PhysicsProcess(float delta)
		{
			
		}

		public override void ExitState(GoblinState newState)
		{
			// player.ReturnNormalGravity();
			player.State = previousState;
		}
	}
}
