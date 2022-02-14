using Godot;
using System;

public class GameManager : Node2D
{
	public Goblin[] PlayerList = new Goblin[4] { null, null, null, null};
	private int newPlayerIndex = 0;
	public Goblin Player { get; private set; }
	private Vector2 screenSize;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Player = GetNodeOrNull<Goblin>("../Goblin");
		screenSize = GetViewport().GetVisibleRect().Size;

		// OS.WindowMaximized = true;
		// OS.WindowFullscreen = !OS.WindowFullscreen;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (Player != null && Player.Position.y > screenSize.y + 50) {
			GetTree().ReloadCurrentScene();
		}

		if (Input.IsActionJustPressed("restart_game")) {
			GetTree().ReloadCurrentScene();
		}

		if (Input.IsActionPressed("toggle_fullscreen")) {
			OS.WindowMaximized = !OS.WindowMaximized;
			OS.WindowFullscreen = !OS.WindowFullscreen;
		}

		if (Input.IsActionJustPressed("quit_game")) {
			GetTree().Quit();
		}
	}

	public void AddNewPlayer(Goblin player) 
	{
		PlayerList[newPlayerIndex % 4] = player;
		newPlayerIndex += 1;
		newPlayerIndex %= 4;
	}
}
