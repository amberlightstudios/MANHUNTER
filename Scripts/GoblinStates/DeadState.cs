using Godot;

namespace GoblinStates
{
	public class DeadState : GoblinState 
	{
		private float timer = 0;

		public DeadState(Goblin player) 
		{
			this.player = player;
			player.Velocity = Vector2.Zero;
			player.ReturnNormalGravity();

			if (!Globals.SinglePlayer) {
				player.SetInvisible();
				player.Killed = true;				
				player.SynchronizeState();
			} else {
				player.GameOver();
			}
		}

		public override void _Process(float delta)
		{

		}

		public override void _PhysicsProcess(float delta)
		{
			player.Velocity.x = 0;
		}

		public override void ExitState(GoblinState newState)
		{
			if (player.IsRevived) {
				player.State = new MoveState(this.player);
				player.Killed = false;
				player.IsRevived = false;
			}
			return;
		}
	}
}
