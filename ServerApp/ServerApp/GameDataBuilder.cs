using GameNetwork;
using Newtonsoft.Json;

namespace ServerApp
{
	internal class GameDataBuilder
	{

		public GameDataBuilder()
		{

		}

		public GameState CreateGameState(int roomId, int roomOwnerId)
		{
			return new GameState()
			{
				playerList = new List<Player>(),
				roomId = roomId,
				roomOwnerId = roomOwnerId
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
