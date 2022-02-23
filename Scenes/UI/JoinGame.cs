using Godot;
using System;

public class JoinGame : Node
{
	private LineEdit NameBox;
	private LineEdit AddressTextBox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		NameBox = (LineEdit) GetNode("MarginContainer/VBoxContainer/MarginContainer2/Name");
 		AddressTextBox = (LineEdit) GetNode("MarginContainer/VBoxContainer/MarginContainer/IP");
		if (NameBox.Text == "") {
			NameBox.GrabFocus();
		} else {
			AddressTextBox.GrabFocus();
		}
		if (Globals.PlayerName != "") {
			NameBox.Text = Globals.PlayerName;
		}
		if (Globals.JoinAddress != "") {
			AddressTextBox.Text = Globals.JoinAddress;
		}
		
	}
	
	private void _on_Name_text_changed(String new_text)
	{
		Globals.PlayerName = new_text;
	}
	
	private void _on_Name_text_entered(String new_text)
	{
		if (new_text == "") return;
		AddressTextBox.GrabFocus();
	}
	
	private void _on_IP_text_changed(String new_text)
	{
		Globals.JoinAddress = new_text;
	}

	private void _on_IP_text_entered(String new_text)
	{
		if (new_text == "") return;
		if (!ValidateIPv4(AddressTextBox.Text) && AddressTextBox.Text.ToLower() != "localhost") {
			Popup popup = (Popup) ((PackedScene) ResourceLoader.Load("res://Scenes/UI/Popup.tscn")).Instance();
			popup.fromJoin = true;
			AddChild(popup);
			RichTextLabel text = (RichTextLabel) popup.GetNode("Panel/RichTextLabel");
			text.Text = "The IP you entered is not valid. Please try a different IP.";
			return;
		}

		if (NameBox.Text == "") {
			Popup popup = (Popup) ((PackedScene) ResourceLoader.Load("res://Scenes/UI/Popup.tscn")).Instance();
			popup.fromJoin = true;
			AddChild(popup);
			RichTextLabel text = (RichTextLabel) popup.GetNode("Panel/RichTextLabel");
			text.Text = "You must enter a name to play.";
			return;
		}

		Globals.HostAddress = AddressTextBox.Text;

		GetTree().ChangeScene(Globals.PathToNetwork);
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













