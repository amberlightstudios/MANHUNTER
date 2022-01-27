using Godot;

namespace BabyStates
{
    public abstract class BabyState
    {
        protected Baby baby;
        public abstract void _Process(float delta);
        public abstract void _PhysicsProcess(float delta);
        public virtual void ExitState(BabyState newState) 
        {
            baby.State = newState;
        }
    }
}