using Godot;
using System;

public class Totem : Area2D
{
	public override void _Ready()
	{

	}

	public override void _Process(float delta)
	{
		
	}

	public override void _PhysicsProcess(float delta) 
	{
		Godot.Collections.Array hitPlayerList = GetOverlappingBodies();
		foreach (Goblin g in hitPlayerList) {
			g.SpawnPos = Position;
		}
	}
}



