using Godot;

namespace GoblinStates
{
	// Can only transition if player is on ground. 
	public class ReviveState : GoblinState
	{
		private Goblin target;
		private float timer = 0f;

		public ReviveState(Goblin player, Goblin deadTeammate) 
		{
			this.player = player;
			target = deadTeammate;
			player.Velocity = Vector2.Zero;
			player.SetZeroGravity();
			target.SetBeingRevivedPuppet(true);
		}

		public override void _Process(float delta)
		{
			if (Input.IsActionJustReleased("Revive")) {
				player.ReturnNormalGravity();
				ExitState(new MoveState(player));
				target.SetBeingRevivedPuppet(false);
				return;
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			timer += delta;
			if (timer > player.ReviveTime) {
				target.IsRevived = true;
				ExitState(new MoveState(player));
			}
		}
	}
}
