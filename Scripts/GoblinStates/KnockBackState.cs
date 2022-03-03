using Godot;

namespace GoblinStates
{
    public class KnockBackState : GoblinState
    {
        private float timer = 0;

        public KnockBackState(Goblin player) 
        {
            this.player = player;
        }

        public override void _Process(float delta)
        {
            
        }

        public override void _PhysicsProcess(float delta)
        {
            timer += delta;
            if (timer > 0.2f) {
                ExitState(new MoveState(player));
            }
        }
    }
}