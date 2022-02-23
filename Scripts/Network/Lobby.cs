using Godot;
using System;
using Godot.Collections;

public class Lobby : Network
{
	public string GameLevel;
	private int ReadyPlayers = 1;
	private bool IsReady;
	private bool CanStart;
	private PackedScene ProfileScene;
	private Label ReadyLabel;
	private string PlayerName;
	private int PlayerId;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProfileScene = (PackedScene) ResourceLoader.Load("res://Scenes/UI/Profile.tscn");
		ReadyLabel = (Label) GetNode("Instructions/HBoxContainer/Tip/Label");

		if (Globals.IsHost) {
			ReadyLabel.Text = "Start";
		} else {
			ReadyLabel.Text = "Ready";	 
		}
		PlayerId = GetTree().GetNetworkUniqueId();
		PlayerName = Globals.PlayerName;	
		AddPlayer(PlayerId, PlayerName);
	}
	
	public override void _Process(float delta)
	{
		CheckCanStart();
		ListenToInput();
	}
	
	public void ListenToInput()
	{
		if (Input.IsActionPressed("Attack")) {
			if (Globals.IsHost) StartGame();
			else ReadyGame();
		} else if (Input.IsActionPressed("Pause")) {
			LeaveGame();
			GetTree().ChangeScene("res://Scenes/UI/Menu.tscn");
		}
	}
	
	public void CheckCanStart()
	{
		if (!CanStart && ReadyPlayers == NumPlayers && ReadyPlayers > 1) {
			CanStart = true;
		}
	}
	
	public void StartGame()
	{
		if (CanStart) {
			CanStart = false;						
			Rpc(nameof(LoadGame));
			LoadLevel();
		}
	}
	
	public void ReadyGame()
	{
		if (!IsReady) {
			IsReady = true;
			Rpc(nameof(ReadyPlayer), PlayerId);			
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
		LoadLevel();
	}
	
	public void AddPlayer(int id, string name) 
	{
		// TODO
		GridContainer container = (GridContainer) GetNode("Players");
		Node user = ProfileScene.Instance();
		Label userName = (Label) user.GetNode("Sprite/VBoxContainer/Name");
		userName.Text = name;
		user.Name = id.ToString();
		container.AddChild(user);
	}
	
	public void RemovePlayer(int id) 
	{
		// TODO
		GridContainer container = (GridContainer) GetNode("Players");
		container.RemoveChild(container.GetNode($"/{id}"));
	}
	

}
