using Godot;
using System;

public class Totem : Area2D
{
	GameManager gm;
	AnimationPlayer animPlayer;
	AudioStreamPlayer2D soundEffect;
	Sprite coin;
	float time = 0f;
	bool faded = false;

	public override void _Ready()
	{
		gm = GetParent().GetNode<GameManager>("GameManager");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animPlayer.Play("Coin");
		coin = GetNode<Sprite>("Coin");
		soundEffect = GetNode<AudioStreamPlayer2D>("SoundEffect");
	}

	public override void _PhysicsProcess(float delta) 
	{
		time += delta;
		coin.Position += new Vector2(0, 0.1f*(Mathf.Sin(4f*time)));
		Godot.Collections.Array hitPlayerList = GetOverlappingBodies();
		foreach (Goblin g in hitPlayerList) {
			g.SpawnPos = Position;
			if (Position.x > gm.TeamSpawnLoc.x) {
				gm.TeamSpawnLoc = Position;
			}
		}

		if (hitPlayerList.Count > 0 && !faded) {
			animPlayer.Play("Fade");
			soundEffect.Play();	
			faded = true;
		}
	}
}



