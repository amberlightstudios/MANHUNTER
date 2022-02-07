using Godot;

namespace ShooterStates
{
    public class StaticState : ShooterState
    {
        private float timer = 0f;

        public StaticState(StaticShooter shooter) 
        {
            this.shooter = shooter;
        }

        public override void _Process(float delta)
        {
            shooter.PlayAnimation("Idle");
        }

        public override void _PhysicsProcess(float delta)
        {
            
        }
    }
}