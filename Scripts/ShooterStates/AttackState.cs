using Godot;
using System;

namespace ShooterStates
{
    public class AttackState : ShooterState
    {
        private Goblin target;
        private float animationLength;
        private float timer = 0f;

        public AttackState(StaticShooter shooter, Goblin target) 
        {
            this.shooter = shooter;
            this.target = target;
            if (target.Position.x < shooter.Position.x) {
                shooter.TurnLeft();
            } else if (target.Position.x > shooter.Position.x) {
                shooter.TurnRight();
            }
            shooter.PlayAnimation("Shoot");
            shooter.Shoot(target);
            animationLength = shooter.AnimPlayer.CurrentAnimationLength;
        }

        public override void _Process(float delta)
        {
            timer += delta;
            if (timer > animationLength + new Random().Next(-1, 2)) {
                ExitState(new StaticState(shooter));
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            
        }
    }
}