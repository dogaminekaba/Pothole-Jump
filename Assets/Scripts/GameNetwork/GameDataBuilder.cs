using GameNetwork;
using System.Collections.Generic;
using System.Linq;

namespace ServerApp
{
	internal class GameDataBuilder
	{
		// Custom deserialization for GameState
		public GameState DeserializeGameState(string data)
		{
			string[] parts = data.Split('|');
			GameState gameState = new GameState();

			gameState.type = parts[0];
			gameState.playerList = parts[1].Split(';').Select(DeserializePlayer).ToList();
			gameState.solidBoxIds = parts[2].Split(',').Select(int.Parse).ToList();
			gameState.roomId = int.Parse(parts[3]);
			gameState.turnPlayerId = int.Parse(parts[4]);

			return gameState;
		}

		// Custom deserialization for Player
		public Player DeserializePlayer(string data)
		{
			string[] parts = data.Split(',');

			Player player = new Player
			{
				type = parts[0],
				id = int.Parse(parts[1]),
				modelId = int.Parse(parts[2]),
				userName = parts[3],
				currentBoxId = int.Parse(parts[4]),
				score = int.Parse(parts[5])
			};

			return player;
		}

		// Custom deserialization for ConnectionRequest
		public ConnectionRequest DeserializeConnectionRequest(string data)
		{
			string[] parts = data.Split(',');

			ConnectionRequest request = new ConnectionRequest
			{
				type = parts[0],
				userName = parts[1],
				modelId = int.Parse(parts[2])
			};

			return request;
		}

		// Custom deserialization for ConnectionResponse
		public ConnectionResponse DeserializeConnectionResponse(string data)
		{
			string[] parts = data.Split(',');

			ConnectionResponse response = new ConnectionResponse
			{
				type = parts[0],
				success = bool.Parse(parts[1]),
				playerId = int.Parse(parts[2]),
				roomId = int.Parse(parts[3])
			};

			return response;
		}

		// Custom deserialization for MoveRequest
		public MoveRequest DeserializeMoveRequest(string data)
		{
			string[] parts = data.Split(',');

			MoveRequest moveRequest = new MoveRequest
			{
				type = parts[0],
				playerId = int.Parse(parts[1]),
				boxId = int.Parse(parts[2]),
				boxColor = int.Parse(parts[3])
			};

			return moveRequest;
		}
	}
}
