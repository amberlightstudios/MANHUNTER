using Godot;
using System;

public class Cam : Camera2D
{
	public Goblin Player { get; set; }

	public override void _Process(float delta)
	{
		if (Player != null) {
			Position = new Vector2(Player.Position.x + 300, Player.Position.y);
		}
	}
}
