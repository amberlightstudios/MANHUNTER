using Godot;
using System;
using System.Threading.Tasks;

public class WinDetect : Area2D
{
	AnimationPlayer animPlayer;
	Sprite coin;
	float time = 0f;
	bool faded = false;

	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animPlayer.Play("Coin");
		coin = GetNode<Sprite>("Coin");
	}

	public override void _PhysicsProcess(float delta)
	{
		time += delta;
		coin.Position += new Vector2(0, 0.1f*(Mathf.Sin(4f*time)));

		if (GetOverlappingBodies().Count > 0 && !faded) {
			faded = true;
			winTransition();
		}
	}
	
	async Task winTransition()
	{
		animPlayer.Play("Fade");
		await Task.Delay(700);
		GetTree().ChangeScene("res://Scenes/UI/WinScreen.tscn");
	}
}
