using Godot;
using System;
using System.Collections.Generic;

public class Level : Node2D
{
	private Generator generator = Generator.Instance;
	
	public override void _Ready()
	{
		if (Globals.SinglePlayer) {
			Goblin player = generator.GeneratePlayer("Single Player", this);;
			AttachCamera(player);
		} else {
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
			}
		}
		FadeOut();
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

	private void FadeOut()
	{
		PackedScene fadeScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Fader.tscn");
		CanvasLayer fader = (CanvasLayer) fadeScene.Instance<CanvasLayer>();
		AnimationPlayer fade = (AnimationPlayer) fader.GetNode("AnimationPlayer");
		fade.PlayBackwards("Fade");
		GetParent().AddChild(fader);
	}
}
