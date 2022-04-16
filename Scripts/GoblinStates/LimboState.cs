using Godot;
using System;

namespace GoblinStates
{
	public class LimboState : GoblinState
	{
		float timer;
		bool startGhost = false;
		Vector2 pos;

		public LimboState(Goblin player) 
		{
			this.player = player;
			player.PlayAnimation("Death");
			pos = player.Position;
			player.Invincible = true;
		}

		public override void _Process(float delta)
		{
			if (player.AnimPlayer.IsPlaying() == false) {
				startGhost = true;
				player.PlayAnimation("Ghost");
			}       
		}

		public override void _PhysicsProcess(float delta)
		{
			player.Position = pos;
			if (startGhost) {
				timer += delta;
				if (timer > 2.8f) {
					player.Position = player.SpawnPos;
					player.Invincible = false;
					ExitState(new MoveState(player));
				}
			}
		}
	}
}
