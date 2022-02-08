using Godot;

namespace BabyStates
{
    public class DeathState : BabyState
    {
        public DeathState(Baby baby) 
        {
            this.baby = baby;
            baby.PlayAnimation("Death");
        }

        public override void _Process(float delta)
        {
            if (!baby.AnimPlayer.IsPlaying()) {
                baby.GetParent().RemoveChild(baby);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            
        }

        public override void ExitState(BabyState newState)
        {
            return;
        }
    }
}