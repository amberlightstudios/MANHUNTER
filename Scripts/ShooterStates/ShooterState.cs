using Godot;

namespace ShooterStates
{
    public abstract class ShooterState 
    {
        protected StaticShooter shooter;

        public abstract void _Process(float delta);
        public abstract void _PhysicsProcess(float delta);
        public virtual void ExitState(ShooterState newState) {
            shooter.State = newState;
        }
    }
}