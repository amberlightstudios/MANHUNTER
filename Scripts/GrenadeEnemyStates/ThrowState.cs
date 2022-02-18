using Godot;

namespace GrenadeEnemyStates
{
    public class ThrowState : GrenadeEnemyState
    {
        private float timer = 0f;
        private Goblin target;

        public ThrowState(GrenadeEnemy enemy, Goblin target) 
        {
            this.enemy = enemy;
            enemy.Velocity = Vector2.Zero;

            this.target = target;
        }

        public override void _Process(float delta)
        {
            timer += delta;
            if (timer > enemy.ThrowFreq) {

            }
            
        }

        public override void _PhysicsProcess(float delta)
        {

        }
    }
}