using Godot;

namespace ShooterStates
{
    public class StaticState : ShooterState
    {
        private float timer = 0f;

        public StaticState(StaticShooter shooter) 
        {
            this.shooter = shooter;
            timer = 0f;
        }

        public StaticState(StaticShooter shooter, float timer)
        {
            this.shooter = shooter;
            this.timer = timer;
        }

        public override void _Process(float delta)
        {
            timer += delta;
            shooter.PlayAnimation("Idle");
        }

        public override void _PhysicsProcess(float delta)
        {
            if (timer > shooter.ShootFrequency) {
                Goblin target = shooter.DetectPlayer();
                if (target != null) {
                    ExitState(new AttackState(shooter, target));
                }
            }
        }
    }
}