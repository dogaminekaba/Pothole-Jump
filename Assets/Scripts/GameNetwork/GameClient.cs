using ServerApp;
using System;
using System.Net.Sockets;
using System.Text;
using UnityEditor.VersionControl;
using UnityEngine.Networking.Types;

namespace GameNetwork
{
	class GameClient
	{
		private int clientId = -1;
		private int roomId = -1;
		private TcpClient? client;
		private NetworkStream stream;

		GameDataBuilder dataBuilder = new GameDataBuilder(); 

		public int Connect(string host, int port, string userName)
		{
			try
			{
				client = new TcpClient(host, port);

				// Create connection request message
				ConnectionRequest connectMsg = new ConnectionRequest
				{
					userName = userName
				};

				// Serialize the message
				string msg = dataBuilder.SerializeMsg(connectMsg);
				Byte[] data = Encoding.ASCII.GetBytes(msg);

				// Get a client stream for reading and writing.
				stream = client.GetStream();

				// Send the message to the connected TcpServer.
				stream.Write(data, 0, data.Length);

				// Receive the server response.
				data = new Byte[256];

				// Read the first batch of the TcpServer response bytes.
				Int32 bytes = stream.Read(data, 0, data.Length);
				string responseData = Encoding.ASCII.GetString(data, 0, bytes);
				Console.WriteLine("Client - Received: ", responseData);

				// Deserialize received message
				ConnectionResponse responseMsg = dataBuilder.DeserializeMsg<ConnectionResponse>(responseData);

				if (responseMsg.playerId > -1)
				{
					clientId = responseMsg.playerId;
				}
				else
				{
					throw new Exception("Server connection problem.");
				}

			}
			catch (ArgumentNullException e)
			{
				Console.WriteLine("Client - ArgumentNullException: ", e);
				return -1;
			}
			catch (SocketException e)
			{
				Console.WriteLine("Client - SocketException: ", e);
				return -1;
			}

			return 0;
		}

		public GameState RequestCreateRoom(int roomSize)
		{
			JoinRoomRequest requestMsg = new JoinRoomRequest()
			{
				createRoom = true,
				roomSize = roomSize,
				roomId = 0
			};
			string msg = dataBuilder.SerializeMsg(requestMsg);

			string response = SendMessage(msg);
			GameState responseMsg = dataBuilder.DeserializeMsg<GameState>(response);

			return responseMsg;
		}


		public int GetClientId()
		{
			return clientId;
		}

		public int GetRoomId()
		{
			return roomId;
		}

		public String SendMessage(String message)
		{
			// String to store the response ASCII representation.
			String responseData = String.Empty;

			try
			{
				// Translate the passed message into ASCII and store it as a Byte array.
				Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

				// Get a client stream for reading and writing.
				NetworkStream stream = client.GetStream();

				// Send the message to the connected TcpServer.
				stream.Write(data, 0, data.Length);

				Console.WriteLine("Client - Sent: ", message);

				// Receive the server response.
				data = new Byte[256];

				// Read the first batch of the TcpServer response bytes.
				Int32 bytes = stream.Read(data, 0, data.Length);
				responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
				Console.WriteLine("Client - Received: ", responseData);
			}
			catch (ArgumentNullException e)
			{
				Console.WriteLine("Client - ArgumentNullException: ", e);
			}
			catch (SocketException e)
			{
				Console.WriteLine("Client - SocketException: ", e);
			}

			return responseData;
		}
	}
}
