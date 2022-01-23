using Godot;
using System;

public class GameManager : Node2D
{
	private Goblin player;
	private Vector2 screenSize;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// player = GetNode<Goblin>("../Goblin");
		screenSize = GetViewport().GetVisibleRect().Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (player != null && player.Position.y > screenSize.y + 50) {
			GetTree().ReloadCurrentScene();
		}

		if (Input.IsActionPressed("toggle_fullscreen")) {
			OS.WindowMaximized = true;
			OS.WindowFullscreen = !OS.WindowFullscreen;
		}
	}
}
