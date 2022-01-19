using Godot;

namespace GoblinStates {
	public abstract class GoblinState
	{
		protected Goblin player;
		public abstract void _Process(float delta);
		public abstract void _PhysicsProcess(float delta);
		public abstract void ExitState(GoblinState newState);

		public override string ToString()
		{
			return this.GetType().Name;
		}
	}
}
