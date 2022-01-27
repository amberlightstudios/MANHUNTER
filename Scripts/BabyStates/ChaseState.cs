using Godot;

namespace BabyStates
{
    public class ChaseState : BabyState
    {
        private Vector2 castto;

        public ChaseState(Baby baby) 
        {
            this.baby = baby;
            castto = baby.PlayerDetect.CastTo;
            baby.PlayerDetect.CastTo = new Vector2(1000, 0);
        }

        public override void _Process(float delta)
        {
            
        }

        private float timer = 3f;
        public override void _PhysicsProcess(float delta)
        {
            if (!baby.PlayerDetect.IsColliding()) {
                timer -= delta;
                if (timer <= 0) {
                    ExitState(new MoveState(baby));
                }
            } else if (timer < 3f) {
                timer = 3f;
            }

            baby.Velocity = new Vector2(baby.Speed * baby.ChaseMultiplier, baby.Velocity.y);
        }

        public override void ExitState(BabyState newState)
        {
            baby.PlayerDetect.CastTo = castto;
            base.ExitState(newState);
        }
    }
}