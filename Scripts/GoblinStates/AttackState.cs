using Godot;
using System;

namespace GoblinStates
{
    public class AttackState : GoblinState
    {
        private GoblinState previousState;

        public AttackState(Goblin player, GoblinState previousState) 
        {
            this.player = player;
            player.Velocity = Vector2.Zero;
            player.AnimPlayer.Play("punch");
            
            this.previousState = previousState;
        }

        public override void _Process(float delta)
        {
            if (player.AnimPlayer.CurrentAnimation != "punch") {
                ExitState(previousState);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            player.AttackEnemy();
        }

        public override void ExitState(GoblinState newState)
        {
            player.State = previousState;
        }
    }
}