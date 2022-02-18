using Godot;

namespace RushEnemyStates
{
    public class AttackState : RushEnemyState
    {
        public AttackState(RushingEnemy enemy) 
        {
            this.enemy = enemy;
        }

        public override void _Process(float delta)
        {
            
        }

        public override void _PhysicsProcess(float delta)
        {
            
        }
    }
}