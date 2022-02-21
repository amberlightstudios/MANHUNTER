using Godot;
using System;

public class Main : Network
{
	private Generator generator = Generator.Instance;
	
	public override void _Ready()
	{
		if (!Globals.SinglePlayer) {
			InitNetwork();
			GD.Print(Globals.HostAddress);
			if (Globals.IsHost) HostGame();
			else JoinGame(Globals.HostAddress);
		} else {
			Goblin player = generator.GeneratePlayer("Single Player", this);;
			AttachCamera(player);
		}
	}
	
	public void AttachCamera(Goblin target) { 
		Cam cam = GetNodeOrNull<Cam>("/root/Main/Cam");
		if (cam != null) {
			cam.Player = target;
		}
	}

}
