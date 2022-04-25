using Godot;
using System;

public class Menu : Control
{
	private bool isMulti = false;
	private Button PlayButton;
	private Button MPButton;
	private Button HostButton;
	private Button JoinButton;
	private Button QuitButton;
	private Button BackButton;

	public override void _Ready()
	{
		PlayButton = (Button)GetNode("MarginContainer/VBoxContainer/Buttons/SP/SP");
		MPButton = (Button)GetNode("MarginContainer/VBoxContainer/Buttons/MP/MP");
		HostButton = (Button)GetNode("MarginContainer/VBoxContainer/Buttons/Host/Host");
		JoinButton = (Button)GetNode("MarginContainer/VBoxContainer/Buttons/Join/Join");
		QuitButton = (Button)GetNode("MarginContainer/VBoxContainer/Buttons/Quit/Quit");
		BackButton = (Button)GetNode("MarginContainer/VBoxContainer/Buttons/Back/Back");
		PlayButton.GrabFocus();
		PlayButton.Connect("pressed", this, nameof(PlayGame));
		MPButton.Connect("pressed", this, nameof(MPGame));
		HostButton.Connect("pressed", this, nameof(HostGame));
		JoinButton.Connect("pressed", this, nameof(JoinGame));
		QuitButton.Connect("pressed", this, nameof(QuitGame));
		BackButton.Connect("pressed", this, nameof(SPGame));
		AnimationPlayer blink = (AnimationPlayer) GetNode("MarginContainer/VBoxContainer/MarginContainer/AnimationPlayer");
		blink.Play("Logo");
		
		SoundToggle toggle = (SoundToggle)GetNode("../SoundToggle");
		if (toggle.isOn)
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Menu"), false);
	}
	
	public override void _Process(float delta) {
		MarginContainer p = (MarginContainer) PlayButton.GetParent();
		MarginContainer m = (MarginContainer) MPButton.GetParent();
		MarginContainer h = (MarginContainer) HostButton.GetParent();
		MarginContainer j = (MarginContainer) JoinButton.GetParent();
		MarginContainer q = (MarginContainer) QuitButton.GetParent();
		MarginContainer b = (MarginContainer) BackButton.GetParent();
		if (!isMulti) {
			p.Visible = true;
			m.Visible = true;
			h.Visible = false;
			j.Visible = false;
			q.Visible = true;
			b.Visible = false;
		} else {
			p.Visible = false;
			m.Visible = false;
			h.Visible = true;
			j.Visible = true;
			q.Visible = false;
			b.Visible = true;
		}
	}

	public void PlayGame() {
		if (GetTree().NetworkPeer != null) {
			((NetworkedMultiplayerENet)GetTree().NetworkPeer).CloseConnection();
			GetTree().NetworkPeer = null;
		}

		Globals.SinglePlayer = true;
		Globals.CurrentLevelMulti = false;
		GetTree().ChangeScene("res://Scenes/UI/LevelSelect.tscn");
	}

	public void MPGame() {
		isMulti = true;
		HostButton.GrabFocus();
	}
	
	public void SPGame() {
		isMulti = false;
		PlayButton.GrabFocus();
	}

	public void HostGame() {
		Globals.SinglePlayer = false;
		Globals.IsHost = true;
		Globals.CurrentLevelMulti = true;
		if (Globals.PlayerName == "Host" || Globals.PlayerName == "") {
			GetTree().ChangeScene("res://Scenes/UI/CreateUser.tscn");
		} else {
			GetTree().ChangeScene("res://Scenes/UI/LevelSelect.tscn");
		}
	}

	public void JoinGame() {
		Globals.SinglePlayer = false;
		Globals.IsHost = false;
		Globals.CurrentLevelMulti = true;
		GetTree().ChangeScene("res://Scenes/UI/JoinGame.tscn");
	}

	public void QuitGame() {
		GetTree().Quit();
	}
}
