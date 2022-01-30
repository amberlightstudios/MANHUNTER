using Godot;

namespace BabyStates
{
    public class AttackState : BabyState 
    {
        public AttackState(Baby baby) 
        {
            this.baby = baby;
            baby.Velocity = new Vector2(baby.GetAttackDist(), -10 * baby.JumpSpeed);
        }

        public override void _Process(float delta)
        {
            
        }

        public override void _PhysicsProcess(float delta)
        {
            if (baby.OnGround() && baby.Velocity.y >= 0) {
                ExitState(new MoveState(baby));
            }   
            
            if (baby.TopDetect.IsColliding() && baby.Velocity.y < 0) {
                baby.Velocity = new Vector2(baby.Velocity.x, 0);
            }
        }
    }
}