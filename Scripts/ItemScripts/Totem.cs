using Godot;
using System;

public class Totem : Area2D
{
	GameManager gm;

	public override void _Ready()
	{
		gm = gm =  GetParent().GetNode<GameManager>("GameManager");
	}

	public override void _Process(float delta)
	{
		
	}

	public override void _PhysicsProcess(float delta) 
	{
		Godot.Collections.Array hitPlayerList = GetOverlappingBodies();
		foreach (Goblin g in hitPlayerList) {
			g.SpawnPos = Position;
			if (Position.x > gm.TeamSpawnLoc.x) {
				gm.TeamSpawnLoc = Position;
			}
		}
	}
}



