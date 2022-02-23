using Godot;

namespace RushEnemyStates 
{
	public abstract class RushEnemyState
	{
		protected RushingEnemy enemy;
		public abstract void _Process(float delta);
		public abstract void _PhysicsProcess(float delta);
		public virtual void ExitState(RushEnemyState newState) 
		{
			enemy.State = newState;
		}
	}
}
