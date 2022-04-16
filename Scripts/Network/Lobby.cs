using Godot;
using System;
using System.Text;
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
	private Label IPBox;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProfileScene = (PackedScene) ResourceLoader.Load("res://Scenes/UI/Profile.tscn");
		ReadyLabel = (Label) GetNode("Instructions/HBoxContainer/Tip/Label");

		if (Globals.IsHost) {
			IPBox = (Label) GetNode("IPBox/Label");
			ReadyLabel.Text = "Start";
			HTTPRequest httpRequest = GetNode<HTTPRequest>("HTTPRequest");
			httpRequest.Connect("request_completed", this, "OnRequestCompleted");
			httpRequest.Request("https://api.ipify.org");
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

	public void OnRequestCompleted(int result, int response_code, string[] headers, byte[] body)
	{
		IPBox.Text += "your public ip: " + System.Text.Encoding.UTF8.GetString(body, 0, body.Length);
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
			ChangePlayerStatus(PlayerId, "Can Start");
		}
	}
	
	public void StartGame()
	{
		if (CanStart) {
			CanStart = false;
			Rpc(nameof(LoadGame));
			LoadLevel();
			QueueFree();
		}
	}
	
	public void ReadyGame()
	{
		if (!IsReady) {
			IsReady = true;
			ChangePlayerStatus(PlayerId, "Ready");
			Rpc(nameof(ReadyPlayer), PlayerId);
		} 
	}
	
	[Remote]
	public void ReadyPlayer(int id)
	{
		ReadyPlayers += 1;
		ChangePlayerStatus(id, "Ready");
	}

	[Remote]
	public void LoadGame()
	{
		LoadLevel();
		QueueFree();
	}
	
	public void AddPlayer(int id, string name) 
	{
		// TODO
		GridContainer container = (GridContainer) GetNode("Players");
		Node user = ProfileScene.Instance();
		user.Name = id.ToString();
		Label userName = (Label) user.GetNode("Sprite/VBoxContainer/Name");
		userName.Text = name;
		container.AddChild(user);
	}
	
	public void RemovePlayer(int id) 
	{
		// TODO
		GridContainer container = (GridContainer) GetNode("Players");
		container.RemoveChild(container.GetNode($"{id}"));
	}
	
	private void ChangePlayerStatus(int id, string status)
	{
		((Label)GetNode($"Players/{id}/Sprite/VBoxContainer/Name2")).Text = status;
	}

	public void LoadLevel()
	{
		Node levelScene = (Node) ((PackedScene) ResourceLoader.Load(Globals.GetPathToLevel(Globals.LevelSelected.ToString()))).Instance();
		GetParent().AddChild(levelScene);
	}
}
