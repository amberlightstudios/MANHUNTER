using Godot;

namespace GoblinStates
{
    public class AttackState : GoblinState
    {
        public AttackState(Goblin player) 
        {
            this.player = player;
        }

        public override void _Process(float delta)
        {
            
        }

        public override void _PhysicsProcess(float delta)
        {
            
        }

        public override void ExitState(GoblinState newState)
        {
            player.State = newState;
        }
    }
}