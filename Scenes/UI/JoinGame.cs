using Godot;
using System;

public class JoinGame : Node
{
	private LineEdit AddressTextBox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
 		AddressTextBox = (LineEdit) GetNode("MarginContainer/VBoxContainer/InputContainer/MarginContainer/LineEdit");
		AddressTextBox.GrabFocus();
	}

	private void _on_LineEdit_text_entered(String new_text)
	{
		// Replace with function body.
		if (!ValidateIPv4(AddressTextBox.Text)) {
			return;
		}

		Globals.HostAddress = AddressTextBox.Text;
		GetTree().ChangeScene("res://Scenes/Main.tscn");
	}

	private bool ValidateIPv4(string ipString)
	{
		if (String.IsNullOrWhiteSpace(ipString))
		{
			return false;
		}

		string[] splitValues = ipString.Split('.');
		if (splitValues.Length != 4)
		{
			return false;
		}

		byte tempForParsing;
		GD.Print(splitValues);

		return true;
	}
}


