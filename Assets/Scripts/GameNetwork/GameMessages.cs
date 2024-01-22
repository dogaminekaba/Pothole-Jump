using System.Collections.Generic;

namespace GameNetwork
{
	[System.Serializable]
	public class GameState
	{
		public List<Player> playerList;
		public int roomId;
		public int roomOwnerId;
		public int turnPlayerId;
		public int roomSize;
	}

	[System.Serializable]
	public class Player
	{
		public int id;
		public int modelId;
		public string userName;
		public int posX;
		public int posY;
		public int score;
	}

	[System.Serializable]
	public class ConnectionRequest
	{
		public string userName;
	}

	[System.Serializable]
	public class ConnectionResponse
	{
		public int playerId;
		public int modelId;
	}

	[System.Serializable]
	public class JoinRoomRequest
	{
		public bool createRoom;
		public int roomId;
		public int roomSize;
	}
}