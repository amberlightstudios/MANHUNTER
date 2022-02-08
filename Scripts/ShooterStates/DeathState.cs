using Godot;

namespace ShooterStates
{
    public class DeathState : ShooterState
    {
        public DeathState(StaticShooter shooter) 
        {
            this.shooter = shooter;
            shooter.PlayAnimation("Death");
        }

        public override void _Process(float delta)
        {
            if (!shooter.AnimPlayer.IsPlaying()) {
                shooter.GetParent().RemoveChild(shooter);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            
        }

        public override void ExitState(ShooterState newState)
        {
            return;
        }
    }
}