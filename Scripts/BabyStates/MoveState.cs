using Godot;
using System;

namespace BabyStates 
{
    public class MoveState : BabyState
    {
        public bool IsChasing = false;

        public MoveState(Baby baby) 
        {
            this.baby = baby;
        }

        public override void _Process(float delta)
        {
            
        }

        public override void _PhysicsProcess(float delta)
        {
            if ((baby.PlayerDetect.IsColliding()
            && Goblin.PlayerType.Equals(baby.PlayerDetect.GetCollider().GetType())) 
            || (baby.PlayerDetectBack.IsColliding() 
            && Goblin.PlayerType.Equals(baby.PlayerDetectBack.GetCollider().GetType())) 
            || IsChasing) {
                if (!IsChasing) {
                    if (baby.PlayerDetect.IsColliding()) {
                        baby.Player = (Goblin) baby.PlayerDetect.GetCollider();
                    } else {
                        baby.Player = (Goblin) baby.PlayerDetectBack.GetCollider();
                    }
                }

                // If player is within range, then jumps him. 
                if (baby.PlayerInAttackRange()) {
                    ExitState(new JumpState(baby));
                    return;
                }

                IsChasing = true;
                baby.Velocity = new Vector2(baby.Speed * Math.Sign(baby.GetAttackDist()), baby.Velocity.y);
                // baby.CheckEdge();
            } else {
                IsChasing = false;
                baby.Velocity = Vector2.Zero;
            }
        }
    }
}