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
	}
	
	private void _on_Name_text_entered(String new_text)
	{
		AddressTextBox.GrabFocus();
	}

	private void _on_IP_text_entered(String new_text)
	{
		if (!ValidateIPv4(AddressTextBox.Text) && AddressTextBox.Text.ToLower() != "localhost") {
			var popup = (PackedScene) ResourceLoader.Load("res://Scenes/UI/Popup.tscn");
			Node node = (Node) popup.Instance();
			AddChild(node);
			RichTextLabel text = (RichTextLabel) node.GetNode("Panel/RichTextLabel");
			text.Text = "The IP you entered is not valid. Please try a different IP.";
			return;
		}

		if (NameBox.Text == "") {
			var popup = (PackedScene) ResourceLoader.Load("res://Scenes/UI/Popup.tscn");
			Node node = (Node) popup.Instance();
			AddChild(node);
			RichTextLabel text = (RichTextLabel) node.GetNode("Panel/RichTextLabel");
			text.Text = "You must enter a name to play.";
			return;
		}

		Globals.HostAddress = AddressTextBox.Text;

		GetTree().ChangeScene("res://Prefabs/Network.tscn");
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




