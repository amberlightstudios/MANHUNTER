using Godot;

namespace GoblinStates {
    public abstract class GoblinState
    {
        protected Vector2 velocity;
        public Vector2 Velocity { get => velocity; set => velocity = value; }
        protected Goblin player;
        public abstract void _Process(float delta);
        public abstract void _PhysicsProcess(float delta);
        public abstract void ExitState(GoblinState newState);
    }
}