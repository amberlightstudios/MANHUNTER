using Godot;

namespace RushEnemyStates
{
    public class KnockBackState : RushEnemyState
    {
        private float timer = 0f;

        public KnockBackState(RushingEnemy enemy) 
        {
            this.enemy = enemy;
            enemy.PlayAnimation(null);
            enemy.BloodGenerator.GenerateBlood(8);
        }

        public override void _Process(float delta)
        {
            
        }

        public override void _PhysicsProcess(float delta)
        {
            timer += delta;

            if (timer > 0.3f) {
                enemy.Velocity = new Vector2(0, enemy.Velocity.y);
                if (timer > 2f) {
                    ExitState(new NormalState(enemy));
                }
            }
        }
    }
}