using Godot;

namespace ShooterStates
{
    public class TakeDamageState : ShooterState
    {
        public TakeDamageState(StaticShooter shooter) 
        {
            this.shooter = shooter;
        }

        public override void _Process(float delta)
        {
            
        }

        public override void _PhysicsProcess(float delta)
        {
            
        }
    }
}