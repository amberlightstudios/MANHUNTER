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
			Enemy enemyGrabbed = player.GrabEnemy();
			if (enemyGrabbed != null) {
				enemyGrabbed.IsGrabbed = true;
				enemyGrabbed.Position = player.ThrowPoint;
				ExitState(new MoveState(player, enemyGrabbed));
			} else {
				ExitState(new MoveState(player));
			}
		}

		public override void ExitState(GoblinState newState)
		{
			player.State = newState;
		}
	}
}
