using System.Collections.Generic;
using System.Linq;

namespace GameNetwork
{
	[System.Serializable]
	public class GameState
	{
		public string type = "GameStateMessage";
		public List<Player> playerList;
		public List<int> solidBoxIds;
		public int roomId;
		public int turnPlayerId;

		// Custom serialization for GameState
		public string Serialize()
		{
			string playerListStr = string.Join(";", playerList.Select(p => p.Serialize()));
			string solidBoxIdsStr = string.Join(",", solidBoxIds);

			return $"{type}|{playerListStr}|{solidBoxIdsStr}|{roomId}|{turnPlayerId}";
		}
	}

	[System.Serializable]
	public class Player
	{
		public string type = "PlayerMessage";
		public int id = -1;
		public int modelId;
		public string userName = "";
		public int currentBoxId;
		public int score;

		// Custom serialization for Player
		public string Serialize()
		{
			return $"{type},{id},{modelId},{userName},{currentBoxId},{score}";
		}
	}

	[System.Serializable]
	public class ConnectionRequest
	{
		public string type = "ConnectionRequestMessage";
		public string userName = "";
		public int modelId;

		// Custom serialization for ConnectionRequest
		public string Serialize()
		{
			return $"{type},{userName},{modelId}";
		}
	}

	[System.Serializable]
	public class ConnectionResponse
	{
		public string type = "ConnectionResponseMessage";
		public bool success;
		public int playerId;
		public int roomId;

		// Custom serialization for ConnectionResponse
		public string Serialize()
		{
			return $"{type},{success},{playerId},{roomId}";
		}
	}

	[System.Serializable]
	public class MoveRequest
	{
		public string type = "MoveRequestMessage";
		public int playerId;
		public int boxId;
		public int boxColor;

		// Custom serialization for MoveRequest
		public string Serialize()
		{
			return $"{type},{playerId},{boxId},{boxColor}";
		}
	}
}