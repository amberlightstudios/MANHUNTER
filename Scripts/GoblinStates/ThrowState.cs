using Godot;

namespace GoblinStates
{
	public class ThrowState : GoblinState
	{        
		public ThrowState(Goblin player) {
			this.player = player;
		}

		public override void _Process(float delta)
		{

		}

		public override void _PhysicsProcess(float delta)
		{
			HeldEnemy = player.GrabEnemy();
			if (HeldEnemy != null) {
				HeldEnemy.IsGrabbed = true;
				HeldEnemy.Position = player.ThrowPoint;
				ExitState(new MoveState(player));
			} else {
				ExitState(new MoveState(player));
			}
		}

		public override void ExitState(GoblinState newState)
		{
			if (HeldEnemy != null) {
				newState.IsHoldingEnemy = true;                
			} else {
				newState.IsHoldingEnemy = false;
			}
			newState.HeldEnemy = HeldEnemy;
			player.State = newState;
		}
	}
}
