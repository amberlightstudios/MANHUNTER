using Godot;

namespace GoblinStates
{
    public class LadderClimbState : GoblinState 
    {
        public LadderClimbState(Goblin player)
        {
            this.player = player;
            player.SetZeroGravity();
            player.PlayAnimation("Ladder");
        }

        public override void _Process(float delta)
        {
            if (Input.IsActionPressed("move_up")) {
                player.Velocity = new Vector2(0, -player.LadderClimbSpeed);
            } else if (Input.IsActionPressed("move_down")) {
                player.Velocity = new Vector2(0, player.LadderClimbSpeed);
            } else {
                player.Velocity = Vector2.Zero;
            }

            if (Input.IsActionPressed("move_left") || Input.IsActionPressed("move_right")) {
                ExitState(new JumpState(player, true));
                return;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            if (!player.OnLadder()) {
                ExitState(new JumpState(player, false));
                return;
            }
        }

        public override void ExitState(GoblinState newState)
        {
            player.ReturnNormalGravity();
            player.State = newState;
        }
    }
}