public static class Globals
{
	public static bool SinglePlayer = true;
	public static bool IsHost = false;
	public static bool LevelsLoaded = false;
	public static int NumLevels = 0;
	public static string PlayerName = "";
	public static string HostAddress = "";
	public static string JoinAddress = "";
	public static int LevelSelected = 1;
	public static string PathToNetwork = "res://Prefabs/Network.tscn";
	
	public static string GetPathToLevel(string level) 
	{
		return $"res://Scenes/Levels/{level}.tscn";
	}
}
