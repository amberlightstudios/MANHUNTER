using Godot;

namespace GoblinStates
{
	public class DeadState : GoblinState 
	{
		private float timer = 0;
		private float animationLength;

		public DeadState(Goblin player) 
		{
			this.player = player;
			player.Velocity = Vector2.Zero;
			player.AnimPlayer.Play("Death");
			animationLength = player.AnimPlayer.CurrentAnimationLength;
		}

		public override void _Process(float delta)
		{
			if (timer > animationLength) {
				if (Globals.SinglePlayer) player.GameOver();
				else {
					player.Killed = true;				
					player.SynchronizeState();
					player.RemoveSelf();					
				}
			}

			timer += animationLength;
			player.AnimPlayer.Play("Death");
		}

		public override void _PhysicsProcess(float delta)
		{
			
		}

		public override void ExitState(GoblinState newState)
		{
			GD.Print("Should not change state from dead state.");
			return;
		}
	}
}
