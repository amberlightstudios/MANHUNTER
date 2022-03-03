using Godot;

namespace ShooterStates
{
    public class KnockBackState : ShooterState
    {
        private float timer = 0f;

        public KnockBackState(StaticShooter shooter) 
        {
            this.shooter = shooter;
            shooter.PlayAnimation(null);
            shooter.BloodGenerator.GenerateBlood(8);
        }

        public override void _Process(float delta)
        {
            
        }

        public override void _PhysicsProcess(float delta)
        {
            timer += delta;
            if (timer > 0.3f) {
                shooter.Velocity = new Vector2(0, shooter.Velocity.y);
                if (timer > 2f) {
                    ExitState(new NormalState(shooter));
                }   
            }
        }
    }
}