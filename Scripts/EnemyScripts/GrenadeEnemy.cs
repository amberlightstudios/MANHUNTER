using Godot;
using System;
using GrenadeEnemyStates;

public class GrenadeEnemy : Enemy
{
	[Export]
	private float throwFreq = 2f;
	public float ThrowFreq { get => throwFreq; }
	[Export]
	private float roamSpeed = 60f;
	public float RoamSpeed { get => roamSpeed; }

	private Area2D playerDetection;

	public GrenadeEnemyState State;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerDetection = GetNode<Area2D>("PlayerDetection");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		State._Process(delta);
	}

	public override void _PhysicsProcess(float delta)
	{
		State._PhysicsProcess(delta);
		base._PhysicsProcess(delta);
	}

	public Goblin PlayerInRange() 
	{
		Godot.Collections.Array targets = playerDetection.GetOverlappingBodies();
		if (targets.Count == 0) {
			return null;
		}
		return (Goblin) targets[0];
	}
}
