using Godot;
using System;

namespace GrenadeEnemyStates
{
    public class NormalState : GrenadeEnemyState
    {
        public NormalState(GrenadeEnemy enemy) 
        {
            this.enemy = enemy;
        }

        public override void _Process(float delta)
        {
            if (enemy.PlayerInRange() != null) {
                
            }
        }

        private int tick = 0;
        public override void _PhysicsProcess(float delta)
        {
            tick++;
            if (tick > 15) {
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
            
            if (enemy.Velocity.x != 0) {
                enemy.PlayAnimation("Walk");
            } else {
                enemy.PlayAnimation("Idle");
            }
        }
    }
}