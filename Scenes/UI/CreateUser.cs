using Godot;
using System;

public class CreateUser : Control
{
	private LineEdit NameBox;
	private AnimationPlayer animPlayer;

	public override void _Ready()
	{
		NameBox = (LineEdit) GetNode("Panel/LineEdit");
		NameBox.GrabFocus();
		
		animPlayer = (AnimationPlayer) GetNode("Panel/AnimationPlayer");
		animPlayer.Play("Popup");
	}

	private void _on_LineEdit_text_changed(String new_text)
	{
		Globals.PlayerName = new_text;
	}
	
	private void _on_LineEdit_text_entered(String new_text)
	{
		Globals.PlayerName = new_text;
		animPlayer.PlayBackwards("Popup");
	}
}

