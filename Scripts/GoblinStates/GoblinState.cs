using Godot;

namespace GoblinStates {
	public abstract class GoblinState
	{
		protected Goblin player;
        public float SpeedBoost = 1;

		public abstract void _Process(float delta);
		public abstract void _PhysicsProcess(float delta);
		public virtual void ExitState(GoblinState newState)
        {
            newState.SpeedBoost = SpeedBoost;
            player.State = newState;
        }

		public override string ToString()
		{
			return this.GetType().Name;
		}
	}
}
