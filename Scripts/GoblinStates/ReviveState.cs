using Godot;
using System;

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
			player.ReviveBar.Visible = true;
			player.NameTag.Visible = false;
			// Tell teammate he is being revived. add method here. 
			target.BeingRevived = true;
		}

		public override void _Process(float delta)
		{
			if (Input.IsActionJustReleased("Revive")) {
				player.ReturnNormalGravity();
				player.ReviveBar.Value = 0;
				player.ReviveBar.Visible = false;
				ExitState(new MoveState(player));
				return;
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			timer += delta;

			GD.Print(Math.Round(timer * 100, 0) * 100);
			if ((Math.Round(timer * 100, 0) * 100) % 50 == 0)  {
				player.ReviveBar.Value += 1;
			}
			if (timer > player.ReviveTime) {
				target.IsRevived = true;
				player.NameTag.Visible = true;
				player.ReviveBar.Visible = false;
				ExitState(new MoveState(player));
			}
		}
	}
}
