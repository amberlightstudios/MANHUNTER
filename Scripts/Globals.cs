public static class Globals
{
	public static bool SinglePlayer = true;
	public static bool IsHost = false;
	public static string PlayerName = "";
	public static string HostAddress = "";
	public static string JoinAddress = "";
	public static string LastPlayedLevel = "";
	public static string PathToNetwork = "res://Prefabs/Network.tscn";
	
	public static string GetPathToLevel(string level) 
	{
		return $"res://Scenes/Levels/{level}.tscn";
	}
}
