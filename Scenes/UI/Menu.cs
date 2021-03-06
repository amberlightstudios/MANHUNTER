using Godot;
using System;

public class Menu : Control
{

	public override void _Ready()
	{
		Button PlayButton = (Button)GetNode("MarginContainer/VBoxContainer/Buttons/MarginContainer1/Play");
		Button HostButton = (Button)GetNode("MarginContainer/VBoxContainer/Buttons/MarginContainer2/Host");
		Button JoinButton = (Button)GetNode("MarginContainer/VBoxContainer/Buttons/MarginContainer3/Join");
		Button QuitButton = (Button)GetNode("MarginContainer/VBoxContainer/Buttons/MarginContainer4/Quit");
		PlayButton.GrabFocus();
		PlayButton.Connect("pressed", this, nameof(PlayGame));
		HostButton.Connect("pressed", this, nameof(HostGame));
		JoinButton.Connect("pressed", this, nameof(JoinGame));
		QuitButton.Connect("pressed", this, nameof(QuitGame));
		
		AnimationPlayer blink = (AnimationPlayer) GetNode("MarginContainer/VBoxContainer/MarginContainer/AnimationPlayer");
		blink.Play("Logo");
	}
	
	public void PlayGame() {
		if (GetTree().NetworkPeer != null) {
			((NetworkedMultiplayerENet)GetTree().NetworkPeer).CloseConnection();
			GetTree().NetworkPeer = null;
		}

		Globals.SinglePlayer = true;
		GetTree().ChangeScene("res://Scenes/UI/LevelSelect.tscn");
	}

	public void HostGame() {
		Globals.SinglePlayer = false;
		Globals.IsHost = true;
		if (Globals.PlayerName == "Host" || Globals.PlayerName == "") {
			GetTree().ChangeScene("res://Scenes/UI/CreateUser.tscn");
		} else {
			GetTree().ChangeScene("res://Scenes/UI/LevelSelect.tscn");
		}
	}
	
	public void JoinGame() {
		Globals.SinglePlayer = false;
		Globals.IsHost = false;
		GetTree().ChangeScene("res://Scenes/UI/JoinGame.tscn");
	}
	
	public void QuitGame() {
		GetTree().Quit();
	}
}
