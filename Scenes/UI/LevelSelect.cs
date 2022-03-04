using Godot;
using System;

public class LevelSelect : Control
{
	int levelSelected = 1;
	public override void _Ready()
	{
		Button L1 = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/1/1");
		Button L2 = (Button) GetNode("MarginContainer/VBoxContainer/Buttons/2/2");
		L1.GrabFocus();
	}
	
	public override void _Process(float delta)
	{
		// TODO: change logic to on enter pressed anywhere inside the main menu and update the state 
	}

	
	private void _on_1_pressed()
	{
		if (Globals.SinglePlayer) 
			GetTree().ChangeScene(Globals.GetPathToLevel("1"));
		else
			GetTree().ChangeScene(Globals.PathToNetwork);
	}


	private void _on_2_pressed()
	{
		if (Globals.SinglePlayer) 
			GetTree().ChangeScene(Globals.GetPathToLevel("2"));
		else
			GetTree().ChangeScene(Globals.PathToNetwork);
	}
	
	private void _on_3_pressed()
	{
		if (Globals.SinglePlayer) {
			GetTree().ChangeScene(Globals.GetPathToLevel("3"));
		} else {
			GetTree().ChangeScene(Globals.PathToNetwork);
		}
	}
}






