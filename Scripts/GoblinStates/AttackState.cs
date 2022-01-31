using Godot;
using System;

namespace GoblinStates
{
	public class AttackState : GoblinState
	{
		private GoblinState previousState;
		private float timer;
		private float animLength;

		public AttackState(Goblin player, GoblinState previousState) 
		{
			this.player = player;
			player.Velocity = Vector2.Zero;
			player.AnimPlayer.Play("Melee1");
			animLength = player.AnimPlayer.CurrentAnimation.Length;
			
			player.AttackEnemy();

			this.previousState = previousState;
		}

		public override void _Process(float delta)
		{
			timer += delta;

			if (timer > animLength) {
				ExitState(previousState);
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
