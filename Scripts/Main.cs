using Godot;
using System;

public class Main : Node2D
{
	private Generator generator = Generator.Instance;
	
	public override void _Ready()
	{
		if (Globals.SinglePlayer) {
			Goblin player = generator.GeneratePlayer("Initial Player", GetNode("/root/Main"));
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
