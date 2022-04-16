using Godot;
using System;

public class LivesUI : CanvasLayer
{
	public AnimationPlayer ap;
	public AnimationPlayer ap1;
	public AnimationPlayer ap2;
	public Goblin player;

	public override void _Ready()
	{
		if (Globals.SinglePlayer) {
			player = GetNode<Goblin>("../Single Player");
		} else {
			player = GetNode<Goblin>($"../{GetTree().GetNetworkUniqueId()}");
		}
		
		ap = GetNode<AnimationPlayer>("Control/GridContainer/Heart/AnimationPlayer");
		ap1 = GetNode<AnimationPlayer>("Control/GridContainer/Heart2/AnimationPlayer");
		ap2 = GetNode<AnimationPlayer>("Control/GridContainer/Heart3/AnimationPlayer");
	}

	public override void _Process(float delta)
	{
		if (player.Lives == 2) {
			ap2.Play("dead");
		} else if (player.Lives == 1) {
			ap1.Play("dead");
		} else if (player.Lives == 0) {
			ap.Play("dead");
		}
	}
}
