using Godot;

namespace RushEnemyStates
{
    public class ChaseState : RushEnemyState
    {
        private Goblin target;

        public ChaseState(RushingEnemy enemy, Goblin target) 
        {
            this.enemy = enemy;
            this.target = target;
            
            enemy.Velocity = new Vector2(target.Position.x - enemy.Position.x, 0).Normalized() * enemy.ChaseSpeed;
            if (enemy.Velocity.x > 0) {
                enemy.TurnRight();
            } else {
                enemy.TurnLeft();
            }
            enemy.PlayAnimation("WalkAlert");
        }

        public override void _Process(float delta)
        {
            
        }

        public override void _PhysicsProcess(float delta)
        {
            if (enemy.PlayerInRange()) {
                ExitState(new AttackState(enemy));
                return;
            }

            if (enemy.EdgeDetect()) {
                ExitState(new NormalState(enemy));
                return;
            }
        }
    }
}