using Godot;

namespace RushEnemyStates
{
	public class DeathState : RushEnemyState
	{
		bool isPlayingAnim = false;

		public DeathState(RushingEnemy enemy) 
		{
			enemy.GetParent().RemoveChild(enemy); // Remove this later
			this.enemy = enemy;
			enemy.Velocity = new Vector2(0, enemy.Velocity.y);
		}

		public override void _Process(float delta)
		{
			if (!isPlayingAnim) {
				// enemy.PlayAnimation("Death");
				isPlayingAnim = true;
			}

			if (isPlayingAnim && !enemy.AnimPlayer.IsPlaying()) {
				enemy.GetParent().RemoveChild(enemy);
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			
		}

		public override void ExitState(RushEnemyState newState)
		{
			return;
		}
	}
}
