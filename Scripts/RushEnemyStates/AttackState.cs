using Godot;

namespace RushEnemyStates
{
    public class AttackState : RushEnemyState
    {
        private float animLength;
        private float timer;

        public AttackState(RushingEnemy enemy) 
        {
            this.enemy = enemy;

            enemy.Velocity = new Vector2(0, enemy.Velocity.y);

            enemy.PlayAnimation("Attack");
            animLength = enemy.AnimPlayer.CurrentAnimationLength;
        }

        public override void _Process(float delta)
        {
            timer += delta;
            if (timer > animLength * 1f/2f) {
                enemy.Attack();
            }

            if (!enemy.AnimPlayer.IsPlaying()) {
                Goblin target = enemy.PlayerDetect();
                if (target != null) {
                    ExitState(new ChaseState(enemy, target));
                } else {
                    ExitState(new NormalState(enemy));
                }
                return;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            
        }
    }
}