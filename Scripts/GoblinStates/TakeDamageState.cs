using Godot;

namespace GoblinStates
{
	public class TakeDamageState : GoblinState 
	{
		private float timer = 0f;

		public TakeDamageState(Goblin player) 
		{
			this.player = player;
			player.Velocity = Vector2.Zero;
		}

		public override void _Process(float delta)
		{
			timer += delta;

			if (timer > player.StunTime) 
			{
				if (player.OnGround()) {
					ExitState(new MoveState(player));
				} else {
					ExitState(new JumpState(player, true));
				}
			} else {
				player.PlayAnimation("Attacked");
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			player.Velocity = new Vector2(player.KnockBackSpeed, player.Velocity.y);
		}

		public override void ExitState(GoblinState newState)
		{
			player.State = newState;
		}
	}
}
