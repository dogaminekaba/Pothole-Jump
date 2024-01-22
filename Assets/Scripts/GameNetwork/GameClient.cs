using ServerApp;
using System;
using System.Net.Sockets;
using System.Text;
using UnityEditor.VersionControl;

namespace GameNetwork
{
	class GameClient
	{
		private int clientId = -1;
		private int roomId = -1;
		TcpClient? client;
		GameDataBuilder gameDataBuilder = new GameDataBuilder(); 

		public int Connect(string host, int port)
		{
			try
			{
				client = new TcpClient(host, port);

				// Create connection request message
				ConnectionRequest connectMsg = new ConnectionRequest
				{
					username = "test1",
					createRoom = true
				};

				// Serialize the message and create a byte array
				string msg = gameDataBuilder.serializeConnectionRequest(connectMsg);
				Byte[] data = Encoding.ASCII.GetBytes(msg);

				// Get a client stream for reading and writing.
				NetworkStream stream = client.GetStream();

				// Send the message to the connected TcpServer.
				stream.Write(data, 0, data.Length);

				// Receive the server response.






				// Buffer to store the response bytes.
				data = new Byte[256];

				// Read the first batch of the TcpServer response bytes.
				Int32 bytes = stream.Read(data, 0, data.Length);
				string responseData = Encoding.ASCII.GetString(data, 0, bytes);
				Console.WriteLine("Client - Received: ", responseData);

				// Deserialize received message
				ConnectionResponse responseMsg = gameDataBuilder.deserializeConnectionResponse(responseData);

				if (responseMsg.playerId > -1)
				{
					clientId = responseMsg.playerId;
				}
				if (responseMsg.roomId > -1)
				{
					roomId = responseMsg.roomId;
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

				// Buffer to store the response bytes.
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
