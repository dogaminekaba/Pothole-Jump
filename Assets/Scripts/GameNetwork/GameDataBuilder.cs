using GameNetwork;
using UnityEngine;

namespace ServerApp
{
	internal class GameDataBuilder
	{
		public GameState lastState;

		public GameDataBuilder() {
			
		}

		public T DeserializeMsg<T>(string jsonString)
		{
			T deserializedData = JsonUtility.FromJson<T>(jsonString);
			return deserializedData;
		}

		public string SerializeMsg<T>(T request)
		{
			string serializedData = JsonUtility.ToJson(request);
			return serializedData;
		}
	}
}
