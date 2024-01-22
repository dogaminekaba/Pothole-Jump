namespace GameNetwork
{
	[System.Serializable]
	public class GameState
	{
		public required List<Player> playerList;
		public int roomId;
		public int roomOwnerId;
		public int turnPlayerId;
	}

	[System.Serializable]
	public class Player
	{
		public int id;
		public string username = "";
		public int posX;
		public int posY;
		public int score;
	}

	[System.Serializable]
	public class ConnectionRequest
	{
		public string username = "";
		public bool createRoom;

	}

	[System.Serializable]
	public class ConnectionResponse
	{
		public int playerId;
		public int roomId;
		public int roomSize;
	}
}