using Godot;

namespace GoblinStates {
    public class MoveState : GoblinState 
    {
        public MoveState(Goblin player) {
            this.player = player;
        }

        public override void _Process(float delta)
        {
            velocity.x = 0;
            if (Input.IsActionPressed("move_left")) {
                velocity.x = -1 * player.Speed;
                player.PlayerSprite.Position = Vector2.Zero;
				player.PlayerSprite.Scale = Vector2.One;
                player.AnimPlayer.Play("Walk");
            } 
            if (Input.IsActionPressed("move_right")) {
                velocity.x = player.Speed;
                player.PlayerSprite.Position = new Vector2(-7, 0);
				player.PlayerSprite.Scale = new Vector2(-1, 1);
				player.AnimPlayer.Play("Walk");
            }

            if (Input.IsActionJustPressed("jump") && velocity.y == 0) {
                velocity.y = -10 * player.JumpSpeed;
            }

            if (velocity.Length() == 0) 
                player.AnimPlayer.Play("Idle");
        }

        public override void _PhysicsProcess(float delta)
        {
            
        }

        public override void ExitState(GoblinState newState)
        {
            player.State = newState;
        }
    }
}