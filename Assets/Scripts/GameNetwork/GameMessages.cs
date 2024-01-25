using System.Collections.Generic;

namespace GameNetwork
{
	[System.Serializable]
	public class GameState
	{
		public string type = "GameState";
		public List<Player> playerList;
		public List<int> solidBoxIds;
		public int roomId;
		public int turnPlayerId;
	}

	[System.Serializable]
	public class Player
	{
		public string type = "Player";
		public int id;
		public int modelId;
		public string userName = "";
		public int posX;
		public int posY;
		public int score;
	}

	[System.Serializable]
	public class ConnectionRequest
	{
		public string type = "ConnectionRequest";
		public string userName = "";
		public int modelId;
	}

	[System.Serializable]
	public class ConnectionResponse
	{
		public string type = "ConnectionResponse";
		public bool success;
		public int playerId;
		public int roomId;
	}

	[System.Serializable]
	public class MoveRequest
	{
		public string type = "MoveRequest";
		public int playerId;
		public int boxId;
	}

	[System.Serializable]
	public class GameStateRequest
	{
		public string type = "GameStateRequest";
		public int roomId;
	}
}