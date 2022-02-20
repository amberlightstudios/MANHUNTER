using Godot;
using System;
using System.Collections.Generic;

public class Network : Node
{
	private readonly int default_port = 4321;

	private string PlayerName { get; set; }

	private Dictionary<int, string> Players = new Dictionary<int, string>();

	private Button HostButton { get; set; }
	private Button JoinButton { get; set; }
	private Button LeaveButton { get; set; }
	private TextEdit NameText { get; set; }
	private TextEdit AddressText { get; set; }
	private Generator generator = Generator.Instance;

	public override void _Ready()
	{
		GetTree().Connect("network_peer_connected", this, nameof(PlayerConnected));
		GetTree().Connect("network_peer_disconnected", this, nameof(PlayerDisconnected));
		GetTree().Connect("connected_to_server", this, nameof(ConnectedToServer));
		GetTree().Connect("connection_failed", this, nameof(ConnectionFailed));
		GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));

		HostButton = (Button)GetNode("VBoxContainer/HostButton");
		HostButton.Connect("pressed", this, nameof(HostGame));

		JoinButton = (Button)GetNode("VBoxContainer/JoinButton");
		JoinButton.Connect("pressed", this, nameof(JoinGame));

		LeaveButton = (Button)GetNode("VBoxContainer/LeaveButton");
		LeaveButton.Connect("pressed", this, nameof(LeaveGame));
		LeaveButton.Disabled = true;

		AddressText = (TextEdit)GetNode("VBoxContainer/AddressText");
		NameText = (TextEdit)GetNode("VBoxContainer/PlayerNameText");
		if (Globals.SinglePlayer) {
			DisableNetwork();
		}
	}


	public void HostGame()
	{
		PlayerName = ((TextEdit)GetNode("VBoxContainer/PlayerNameText")).Text;

		var peer = new NetworkedMultiplayerENet();
		peer.CreateServer(default_port, 32);
		GetTree().NetworkPeer = peer;
		
		GD.Print("You are now hosting.");

		SetInGame();
		StartGame();
	}

	public void JoinGame()
	{
		var address = AddressText.Text;

		GD.Print($"Joining game with address {address}");

		var clientPeer = new NetworkedMultiplayerENet();
		var result = clientPeer.CreateClient(address, default_port);

		GetTree().NetworkPeer = clientPeer;
	}

	public void LeaveGame()
	{
		GD.Print("Leaving current game");

		foreach(var player in Players)
		{
			GetNode(player.Key.ToString()).QueueFree();
		}

		Players.Clear();

		GetNode(GetTree().GetNetworkUniqueId().ToString()).QueueFree();

		Rpc(nameof(RemovePlayer), GetTree().GetNetworkUniqueId());

		((NetworkedMultiplayerENet)GetTree().NetworkPeer).CloseConnection();
		GetTree().NetworkPeer = null;

		SetInMenu();
	}

	private void PlayerConnected(int id)
	{
		PlayerName = NameText.Text;

		GD.Print($"tell other player my name is {PlayerName} and my id is {id}");
		// tell the player that just connected who we are by sending an rpc back to them with your name.
		RpcId(id, nameof(RegisterPlayer), PlayerName);
	}

	private void PlayerDisconnected(int id)
	{
		GD.Print("Player disconnected");
		RemovePlayer(id);
	}

	private void ConnectedToServer()
	{

		GD.Print("Successfully connected to the server");

		SetInGame();
		StartGame();
	}

	private void ConnectionFailed()
	{
		GetTree().NetworkPeer = null;

		GD.Print("Failed to connect.");

		SetInMenu();
	}

	private void ServerDisconnected()
	{
		GD.Print($"Disconnected from the server");

		SetInMenu();
		LeaveGame();
	}

	[Remote]
	private void RegisterPlayer(string playerName)
	{
		var id = GetTree().GetRpcSenderId();

		Players.Add(id, playerName);

		GD.Print($"{playerName} added with ID {id}");

		// a player has been added spawn in the right location
		Goblin peerPlayer = SpawnPlayer(id, playerName);
		peerPlayer.SetColor(new Color(1, 0.39f, 0.28f, 1));
	}
	
	public void AttachCamera(Goblin target) { 
		Cam cam = GetNodeOrNull<Cam>("/root/Main/Cam");
		if (cam != null) {
			cam.Player = target;
		}
	}

	[Remote]
	private void StartGame()
	{
		Goblin player = SpawnPlayer(GetTree().GetNetworkUniqueId(), PlayerName);
		AttachCamera(player);
	}

	private Goblin SpawnPlayer(int id, string playerName)
	{
		Goblin playerNode = generator.GeneratePlayer(id.ToString(), GetNode("/root/Main"));;
		playerNode.SetNetworkMaster(id);
		GetNode("/root/Main").AddChild(playerNode);
		return playerNode;
	}

	[Remote]
	private void RemovePlayer(int id)
	{

		if (Players.ContainsKey(id))
		{
			Players.Remove(id);
			GetNode(id.ToString()).QueueFree();
		}
	}

	// Lock the fields the player shouldn't use while in a game
	private void SetInGame()
	{
		HostButton.Disabled = true;
		JoinButton.Disabled = true;
		LeaveButton.Disabled = false;
		NameText.Readonly = true;
		AddressText.Readonly = true;
	}

	// unlock the fields the player should be able to use while not in game
	private void SetInMenu()
	{
		HostButton.Disabled = false;
		JoinButton.Disabled = false;
		LeaveButton.Disabled = true;
		NameText.Readonly = false;
		AddressText.Readonly = false;
	}
	
	private void DisableNetwork()
	{
		HostButton.Disabled = true;
		JoinButton.Disabled = true;
		LeaveButton.Disabled = true;
		NameText.Readonly = true;
		AddressText.Readonly = true;
	}
}