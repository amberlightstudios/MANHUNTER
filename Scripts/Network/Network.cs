using Godot;
using System;
using System.Collections.Generic;

public class Network : Node
{
	private readonly int default_port = 4321;

	public string PlayerName { get; set; }
	public int PlayerId { get; set; }
	public Dictionary<int, string> Players = new Dictionary<int, string>();
	private TextEdit NameText { get; set; }
	public int NumPlayers = 1;
	private Generator generator = Generator.Instance;
	private Lobby LobbyRoom;
	public override void _Ready()
	{
		LobbyRoom = (Lobby) ((PackedScene) ResourceLoader.Load("res://Scenes/UI/Lobby.tscn")).Instance();
		GetTree().Connect("network_peer_connected", this, nameof(PlayerConnected));
		GetTree().Connect("network_peer_disconnected", this, nameof(PlayerDisconnected));
		GetTree().Connect("connected_to_server", this, nameof(ConnectedToServer));
		GetTree().Connect("connection_failed", this, nameof(ConnectionFailed));
		GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));
		if (Globals.IsHost) {
			Globals.PlayerName = "Host";
			HostGame();
		} else {
			JoinGame(Globals.HostAddress);
		}
		PlayerId = GetTree().GetNetworkUniqueId();		
		PlayerName = Globals.PlayerName;
		Players.Add(PlayerId, PlayerName);
	}

	public void HostGame()
	{
		var peer = new NetworkedMultiplayerENet();
		peer.CreateServer(default_port, 32);
		GetTree().NetworkPeer = peer;
		GD.Print("You are now hosting.");	
		JoinLobby();
	}

	public void JoinGame(string address)
	{
		GD.Print($"Joining game with address {address}");

		var clientPeer = new NetworkedMultiplayerENet();
		var result = clientPeer.CreateClient(address, default_port);

		GetTree().NetworkPeer = clientPeer;
	}

	public void LeaveGame()
	{
		GD.Print("Leaving current game");

		Players.Clear();

		((NetworkedMultiplayerENet)GetTree().NetworkPeer).CloseConnection();
		GetTree().NetworkPeer = null;

	}

	private void PlayerConnected(int id)
	{
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
		JoinLobby();	
	}

	private void ConnectionFailed()
	{
		GetTree().NetworkPeer = null;

		GD.Print("Failed to connect.");
		GetTree().ChangeScene("res://Scenes/UI/Popup.tscn");
	}

	private void ServerDisconnected()
	{
		GD.Print($"Disconnected from the server");

		LeaveGame();
	}

	[Remote]
	private void RegisterPlayer(string playerName)
	{
		var id = GetTree().GetRpcSenderId();
		Players.Add(id, playerName);
		GD.Print($"{playerName} added with ID {id}");
		LobbyRoom.AddPlayer(id, playerName);	
		NumPlayers += 1;	
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

	public Goblin SpawnPlayer(int id, string playerName)
	{
		Goblin playerNode = generator.GeneratePlayer(id.ToString(), GetNode("/root/Main"));;
		playerNode.SetNetworkMaster(id);
		return playerNode;
	}

	[Remote]
	private void RemovePlayer(int id)
	{

		if (Players.ContainsKey(id))
		{
			Players.Remove(id);
		}
		LobbyRoom.RemovePlayer(id);
		NumPlayers -= 1;
	}
	
	private void JoinLobby()
	{
		RemoveChild(GetNode("Loading"));
		AddChild(LobbyRoom);
	}
	
	public void LoadLevel()
	{
		Node levelScene = (Node) ((PackedScene) ResourceLoader.Load(
							Globals.GetPathToLevel(Globals.LastPlayedLevel))
						).Instance();
		GetNode("Lobby").QueueFree();
		AddChild(levelScene);		
	}
}
