using GameNetwork;
using Newtonsoft.Json;

namespace ServerApp
{
	internal class GameDataBuilder
	{

		public GameDataBuilder()
		{

		}

		public GameState CreateGameState(int roomId, int boardSize)
		{
			int startIndex = 0;
			int endIndex = boardSize;
			List<int> solidBoxIds = new List<int>();
			Random random = new Random();

			for (int i = 0; i < boardSize; i++)
			{
				int median = (startIndex + endIndex) / 2;

				int solid1 = random.Next(startIndex, median) + 1;
				int solid2 = random.Next(median, endIndex) + 1;

				solidBoxIds.Add(solid1);
				solidBoxIds.Add(solid2);

				startIndex += boardSize;
				endIndex += boardSize;
			}

			return new GameState()
			{
				playerList = new List<Player>(),
				solidBoxIds = solidBoxIds,
				turnPlayerId = -10,
				roomId = roomId
			};
		}

		public GameState AddPlayer(GameState state, int playerId)
		{
			state.playerList.Add(new Player
			{
				id = playerId,
				userName = "Player " + playerId,
				posX = -playerId,
				posY = -playerId,
				score = 0
			});

			return state;
		}

		public T? DeserializeMsg<T>(string jsonString)
		{
			T deserializedData = JsonConvert.DeserializeObject<T>(jsonString);
			return deserializedData;
		}

		public string SerializeMsg<T>(T request)
		{
			string serializedData = JsonConvert.SerializeObject(request);
			return serializedData;
		}
	}
}
