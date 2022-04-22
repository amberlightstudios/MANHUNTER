using Godot;
using System;

public class GameManager : Node2D
{
	public Goblin[] PlayerList = new Goblin[4] { null, null, null, null};
	public Goblin[] StaticPlayerList = new Goblin[4] { null, null, null, null };
	public Goblin[] LivePlayerList = new Goblin[4] { null, null, null, null };
	
	public int NumPlayers = 0;
	public int LivePlayers = 0;	
	private int newPlayerIndex = 0;

	[Export]
	public int TeamLives = 2;
	public Vector2 TeamSpawnLoc;
	
	[Puppet] public Vector2 PuppetTeamSpawnLoc { get; set; }
	[Puppet] public int PuppetTeamLives = 2;
	
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
		
		if (!Globals.SinglePlayer) {
			SynchronizeState();
		}
	}

	public void SynchronizeState()
	{
		if (GetTree().IsNetworkServer()) {
			RpcUnreliable(nameof(UpdateState), TeamSpawnLoc, TeamLives);		
		} else {
			ReceiveState();
		}
	}

	[Remote]
	public void UpdateState(Vector2 loc, int lives)
	{
		PuppetTeamSpawnLoc = loc;
		PuppetTeamLives = lives;
	}

	public void ReceiveState()
	{
		TeamSpawnLoc = PuppetTeamSpawnLoc;
		if (PuppetTeamLives < TeamLives) {
			TeamReset();
		}		
		TeamLives = PuppetTeamLives;
	}

	public int AddNewPlayer(Goblin player) 
	{
		GD.Print($"Adding Player {player.PlayerName}");
		int oldIndex = newPlayerIndex;
		while (PlayerList[newPlayerIndex] != null) {
			++newPlayerIndex;
			if (newPlayerIndex % 4 == oldIndex) {
				GD.Print("Cannot find open spot for new player");
				return -1;
			}
		}
		PlayerList[newPlayerIndex] = player;
		StaticPlayerList[newPlayerIndex] = player;
		LivePlayerList[newPlayerIndex] = player;
		newPlayerIndex += 1;
		NumPlayers += 1;
		LivePlayers += 1;
		return newPlayerIndex - 1;
	}
	
	public void SetNewPlayer(Goblin player, int index)
	{
		if (PlayerList[index] == null) {
			NumPlayers += 1;
			PlayerList[index] = player;
			GD.Print($"Set Player {player.PlayerName}. Now Number Is {NumPlayers}");
		}
	}
	
	public void RemovePlayer(int index) 
	{	
		if (PlayerList[index] == null) {
			return;
		}
		NumPlayers -= 1;
		GD.Print($"Remove Player {index}. Now Number Is {NumPlayers}");		
		PlayerList[index] = null;
	}
	
	public Goblin GetRandomAlive()
	{
		foreach (Goblin player in PlayerList) {
			if (player != null) return player;
		}	
		return null;
	}

	public void TeamReset() 
	{
		GD.Print($"Team Resetting with {TeamLives} lives left");
		TeamLives -= 1;
		int index = 0;
		foreach (Goblin g in StaticPlayerList) {
			if (g != null) {
				GD.Print($"Respawning {g.PlayerName}");
				SetNewPlayer(g, index);		
				AddLivePlayer(g, index);		
				g.Respawn();
				index++;
			}
		}
	}
	
	public void RemoveLivePlayer(int index)
	{
		if (LivePlayerList[index] == null) return;
		LivePlayers -= 1;
		LivePlayerList[index] = null;
		if (LivePlayers == 0) {
			if (TeamLives == 0) 
				GameOver();
			else if (GetTree().IsNetworkServer())
				TeamReset();
		}
	}
	
	public void AddLivePlayer(Goblin goblin, int index)
	{
		if (LivePlayerList[index] != null) return;
		LivePlayerList[index] = goblin;
		LivePlayers += 1;
	}
	
	async public void GameOver()
	{
		GD.Print("Game Over called in GM");
		int wait = 1;
		Timer t = new Timer();
		t.SetWaitTime(3);
		t.SetOneShot(true);
		AddChild(t);
		t.Start();
		await ToSignal(t, "timeout");
		Network network = GetNode<Network>("/root/Network");	
		network.LeaveGame();
	}
}
