using Godot;
using System;
using System.Collections.Generic;

public class Level : Network
{
	private Generator generator = Generator.Instance;
	Node livesUI = (Node) ((PackedScene) ResourceLoader.Load("res://Scenes/UI/LivesUI.tscn")).Instance();
	
	public override void _Ready()
	{
		if (Globals.SinglePlayer) {
			Goblin player = generator.GeneratePlayer("Single Player", this);;
			AttachCamera(player);
			AddChild(livesUI);
		} else {
			// Skins
			Texture Kanye = ResourceLoader.Load("res://Sprites/GoblinSkins/GoblinK.png") as Texture;
			Texture Ukraine = ResourceLoader.Load("res://Sprites/GoblinSkins/GoblinU.png") as Texture;
			Texture USA = ResourceLoader.Load("res://Sprites/GoblinSkins/GoblinUSA.png") as Texture;
			bool k = true;
			bool u = true;
			bool usa = true;
	
			Goblin LastPlayer = null;
			Dictionary<int, string> Players = ((Network)GetParent()).Players;
			foreach( KeyValuePair<int, string> kvp in Players)
			{
				Goblin Player = SpawnPlayer(kvp.Key, kvp.Value);
				if (kvp.Key == GetTree().GetNetworkUniqueId()) {
					AttachCamera(Player);
				}
				if (LastPlayer != null) {
					Player.Position = new Vector2(LastPlayer.Position.x, LastPlayer.Position.y);
				}
				LastPlayer = Player;
				
				if (k) {
					Player.PlayerSprite.SetTexture(Kanye);
					k = false;
					continue;
				} else if (u) {
					Player.PlayerSprite.SetTexture(Ukraine);
					u = false;
					continue;
				} else if (usa) {
					Player.PlayerSprite.SetTexture(USA);
					usa = false;
					continue;
				}
			}
		}
	}
	
	public void AttachCamera(Goblin target) { 
		Cam cam = (Cam) GetNode("Cam");
		if (cam != null) {
			cam.Player = target;
		}
	}
	
	public Goblin SpawnPlayer(int id, string playerName)
	{
		Goblin playerNode = generator.GeneratePlayer(id.ToString(), this);
		playerNode.SetName(playerName);
		playerNode.SetNetworkMaster(id);
		return playerNode;
	}

}
