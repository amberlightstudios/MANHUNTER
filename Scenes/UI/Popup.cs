using Godot;
using System;
using System.Threading.Tasks;

public class Popup : Control
{
	public bool fromJoin = false;
	public string text = "";
	
	public RichTextLabel label;
	private AnimationPlayer animPlayer;

	public override void _Ready()
	{
		label = (RichTextLabel) GetNode("Panel/RichTextLabel");
		if (label.Text == "") label.Text = text;

		animPlayer = (AnimationPlayer) GetNode("Panel/AnimationPlayer");
		animPlayer.Play("Popup");
		Button dismiss = (Button) GetNode("Panel/Dismiss");
		dismiss.GrabFocus();
	}

	private void _on_Dismiss_pressed()
	{
		Dismiss();
	}

	async Task Dismiss()
	{
		animPlayer.PlayBackwards("Popup");
		await Task.Delay(500);
		if (fromJoin) {
			GetTree().ChangeScene("res://Scenes/UI/JoinGame.tscn");
		} else {
			GetTree().ChangeScene("res://Scenes/UI/Menu.tscn");
		}
	}
}


