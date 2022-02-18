using Godot;

namespace RushEnemyStates
{
    public class ChaseState : RushEnemyState
    {
        public ChaseState(RushingEnemy enemy) 
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