using Godot;
using System;

namespace Generator {
	public class Generator
	{
		// Singleton pattern
	   private static Generator instance;

	   private Generator() {}

	   public static Generator Instance
	   {
		  get 
		  {
			 if (instance == null)
			 {
				instance = new Generator();
			 }
			 return instance;
		  }
	   }
	
		public Goblin GeneratePlayer(string name, Node target) {
			var playerScene = (PackedScene)ResourceLoader.Load("res://Prefabs/Goblin.tscn");
			Goblin playerNode = playerScene.Instance<Goblin>();
			playerNode.Name = name;
			target.AddChild(playerNode);
			return playerNode;
		}
		


	}

}
