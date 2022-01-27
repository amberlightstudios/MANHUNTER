using Godot;
using System;

namespace BabyStates 
{
    public class MoveState : BabyState
    {
        public MoveState(Baby baby) 
        {
            this.baby = baby;
        }

        public override void _Process(float delta)
        {
            
        }

        public override void _PhysicsProcess(float delta)
        {
            if (baby.PlayerDetect.IsColliding()
            && Goblin.PlayerType.Equals(baby.PlayerDetect.GetCollider().GetType())) {
                ExitState(new ChaseState(baby));
            }

            baby.Velocity = new Vector2(baby.Speed, baby.Velocity.y);
        }
    }
}