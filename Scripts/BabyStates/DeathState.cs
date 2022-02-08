using Godot;

namespace BabyStates
{
    public class DeathState : BabyState
    {
        bool isPlayingAnim = false;
        
        public DeathState(Baby baby) 
        {
            this.baby = baby;
            baby.Velocity = new Vector2(0, baby.Velocity.y);
        }

        public override void _Process(float delta)
        {
            if (baby.OnGround() && !isPlayingAnim) {
                baby.PlayAnimation("Death");
                isPlayingAnim = true;
            }

            if (isPlayingAnim && !baby.AnimPlayer.IsPlaying()) {
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