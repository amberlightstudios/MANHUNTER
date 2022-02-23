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
            player.SetCollisionLayerBit(1, false);
            player.SetCollisionLayerBit(7, true);
		}

		public override void _Process(float delta)
		{
			if (Globals.SinglePlayer) {
				player.GameOver();
			} else {
				player.Killed = true;				
				player.SynchronizeState();
				player.RemoveSelf();					
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			player.Velocity.x = 0;
		}

		public override void ExitState(GoblinState newState)
		{
			if (player.IsRevived) {
                player.State = new MoveState(this.player);
                player.IsRevived = false;
            }
			return;
		}
	}
}
