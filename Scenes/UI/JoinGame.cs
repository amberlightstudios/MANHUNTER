using Godot;
using System;

public class JoinGame : Node
{
	private LineEdit AddressTextBox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
 		AddressTextBox = (LineEdit) GetNode("MarginContainer/VBoxContainer/InputContainer/MarginContainer/LineEdit");
	}

	private void _on_LineEdit_text_entered(String new_text)
	{
		// Replace with function body.
		Globals.HostAddress = AddressTextBox.Text;
		GetTree().ChangeScene("res://Scenes/Main.tscn");
	}

}


