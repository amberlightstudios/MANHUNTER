using Godot;
using System;

public class WinDetect : Area2D
{
	public override void _PhysicsProcess(float delta)
	{
		if (GetOverlappingBodies().Count > 0) {
			// Change to winning scene. 
			GetTree().ChangeScene("res://Scenes/UI/WinScreen.tscn");
		}
	}
}
