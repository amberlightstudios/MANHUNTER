using Godot;
using System;

namespace GoblinStates
{
	public class AttackState : GoblinState
	{
		private GoblinState previousState;
		private float timer = 0;
		private float animLength;

		public AttackState(Goblin player, GoblinState previousState) 
		{
			this.player = player;
			player.Velocity = Vector2.Zero;
			player.AnimPlayer.Play("Melee1");
			animLength = player.AnimPlayer.CurrentAnimationLength;
            GD.Print(animLength);
			
			player.AttackEnemy();

			this.previousState = previousState;
		}

		public override void _Process(float delta)
		{
			timer += delta;

			if (timer > animLength) {
				ExitState(previousState);
				return;
			}

			player.AnimPlayer.Play("Melee1");
		}

		public override void _PhysicsProcess(float delta)
		{
			
		}

		public override void ExitState(GoblinState newState)
		{
			player.State = previousState;
		}
	}
}
