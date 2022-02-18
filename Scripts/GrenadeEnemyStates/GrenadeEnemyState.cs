using Godot;

namespace GrenadeEnemyStates 
{
    public abstract class GrenadeEnemyState
    {
        protected GrenadeEnemy enemy;
        public abstract void _Process(float delta);
		public abstract void _PhysicsProcess(float delta);
		public virtual void ExitState(GrenadeEnemyState newState) {
			enemy.State = newState;
		}
    }
}