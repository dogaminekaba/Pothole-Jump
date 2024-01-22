using GameNetwork;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

namespace ServerApp
{
	internal class GameDataBuilder
	{
		public GameState lastState;

		public GameDataBuilder() {

			//List<Player> playerList = new List<Player>()
			//{
			//	new Player()
			//	{
			//		id = roomOwnerId,
			//		username = "Player " + roomOwnerId,
			//		posX = -1,
			//		posY = -1,
			//		score = 0
			//	}
			//};
			
			////lastState = new GameState()
			////{
			////	Players = { playerList },
			////	RoomNumber = roomId,
			////	RoomOwnerId = roomOwnerId
			////};
			
		}

		public void addPlayer(int playerId)
		{
			//lastState.Players.Add(new Player
			//{
			//	Id = playerId,
			//	Username = "Player " + playerId,
			//	PosX = -playerId,
			//	PosY = -playerId,
			//	Score = 0
			//});
		}

		public ConnectionRequest deserializeConnectionRequest(string jsonString)
		{
			ConnectionRequest deserializedData = JsonUtility.FromJson<ConnectionRequest>(jsonString);
			return deserializedData;
		}

		public string serializeConnectionRequest(ConnectionRequest request)
		{
			string serializedData = JsonUtility.ToJson(request);
			return serializedData;
		}

		public ConnectionResponse deserializeConnectionResponse(string jsonString)
		{
			ConnectionResponse deserializedData = JsonUtility.FromJson<ConnectionResponse>(jsonString);
			return deserializedData;
		}

		public string serializeConnectionResponse(ConnectionResponse request)
		{
			string serializedData = JsonUtility.ToJson(request);
			return serializedData;
		}

		//public byte[] serializeGameState(GameState state)
		//{
		//	byte[] serializedData = state.ToByteArray();
		//	return serializedData;
		//}

		//public GameState deserializeGameState(byte[] message)
		//{
		//	GameState deserializedData = GameState.Parser.ParseFrom(message);
		//	return deserializedData;
		//}
	}
}
