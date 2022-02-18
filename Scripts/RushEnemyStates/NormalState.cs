using Godot;
using System;

namespace RushEnemyStates
{
    public class NormalState : RushEnemyState
    {
        public NormalState(RushingEnemy enemy) 
        {
            this.enemy = enemy;
        }

        public override void _Process(float delta)
        {
            if (enemy.Velocity.x != 0) {
                enemy.PlayAnimation("Walk");
            } else {
                enemy.PlayAnimation("Idle");
            }
        }

        private int tick = 0;
        public override void _PhysicsProcess(float delta)
        {
            Goblin target = enemy.PlayerDetect();
            if (target != null) {
                ExitState(new ChaseState(enemy, target));
                return;
            }

            tick++;
            if (tick > 20) {
                int random = new Random().Next(-1, 5);
                tick = 0;
                if (Math.Abs(random) == 1) {
                    enemy.Velocity = new Vector2(enemy.RoamSpeed * random, enemy.Velocity.y);
                    if (random < 0) {
                        enemy.TurnLeft();
                    } else {
                        enemy.TurnRight();
                    }
                } else {
                    enemy.Velocity = new Vector2(0, enemy.Velocity.y);
                }
            }

            enemy.EdgeDetect();
        }
    }
}