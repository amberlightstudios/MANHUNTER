using Godot;

namespace GoblinStates
{
	public class ThrowState : GoblinState
	{
		public ThrowState(Goblin player) {
			this.player = player;
		}

		public override void _Process(float delta)
		{
			if (Input.IsActionJustPressed("Grab")) {
				
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			throw new System.NotImplementedException();
		}

		public override void ExitState(GoblinState newState)
		{
			throw new System.NotImplementedException();
		}
	}
}
