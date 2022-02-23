using Godot;
using System;
using Godot.Collections;

public class Lobby : Network
{
	public string GameLevel;
	public Button StartButton;
	public Button ReadyButton;
	private int ReadyPlayers;
	private bool IsReady;
	private bool CanStart;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Button btn = (Button) GetNode("Button");
		if (Globals.IsHost) {
			btn.Text = "Start Game";
			btn.Connect("pressed", this, nameof(StartGame));
			StartButton = btn;
		} else {
			btn.Text = "Ready";	 
			btn.Connect("pressed", this, nameof(ReadyGame));			
			ReadyButton = btn;
		}
		AddPlayer(PlayerId, PlayerName);
	}
	
	public override void _Process(float delta)
	{
		CheckCanStart();
	}
	
	public void CheckCanStart()
	{
		if (!CanStart && ReadyPlayers == NumPlayers) {
			StartButton.Disabled = false;
			CanStart = true;
		}
	}
	
	public void StartGame()
	{
		if (CanStart) {
			StartButton.Disabled = true;
			Rpc(nameof(LoadGame));
			GetTree().ChangeScene(GameLevel);
			CanStart = false;			
		}
	}
	
	public void ReadyGame()
	{
		if (!IsReady) {
			IsReady = true;
			Rpc(nameof(ReadyPlayer), PlayerId);
			ReadyButton.Disabled = true;
		} 
	}
	
	[Remote]
	public void ReadyPlayer()
	{
		ReadyPlayers += 1;
	}

	[Remote]
	public void LoadGame()
	{
		GetTree().ChangeScene(GameLevel);
	}
	
	public void AddPlayer(int id, string name) {
		// TODO
	}
	
	public void RemovePlayer(int id) {
		// TODO
	}
}
