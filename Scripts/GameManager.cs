using Godot;
using System;

public class GameManager : Node2D
{
	public Goblin[] PlayerList = new Goblin[4] { null, null, null, null};
	public int NumPlayers = 0;
	private int newPlayerIndex = 0;
	public Goblin Player { get; private set; }
	public int NumEnemies = 0;
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
	}

	public int AddNewPlayer(Goblin player) 
	{
        int oldIndex = newPlayerIndex;
        while (PlayerList[newPlayerIndex] != null) {
            ++newPlayerIndex;
            if (newPlayerIndex % 4 == oldIndex) {
                GD.Print("Cannot find open spot for new player");
                return -1;
            }
        }
		PlayerList[newPlayerIndex] = player;
		newPlayerIndex += 1;
		NumPlayers += 1;
		// newPlayerIndex %= 4;
		return newPlayerIndex - 1;
	}
	
	public void SetNewPlayer(Goblin player, int index)
	{
		if (PlayerList[index] == null) {
			NumPlayers += 1;
			PlayerList[index] = player;
		}
	}
	
	public void RemovePlayer(int index) 
	{	
		if (PlayerList[index] == null) {
			return;
		}
		NumPlayers -= 1;
		PlayerList[index] = null;
	}
	
	public Goblin GetRandomAlive()
	{
		foreach (Goblin player in PlayerList) {
			if (player != null) return player;
		}	
		return null;
	}
}
