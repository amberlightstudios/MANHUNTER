using Godot;

namespace BabyStates
{
    public class AttackState : BabyState 
    {
        public AttackState(Baby baby) 
        {
            this.baby = baby;
            baby.Velocity = new Vector2(baby.GetAttackDist() * 0.7f, -10 * baby.JumpSpeed);
        }

        public override void _Process(float delta)
        {
            
        }

        public override void _PhysicsProcess(float delta)
        {
            if (baby.OnGround()) {
                ExitState(new MoveState(baby));
            }   
        }
    }
}