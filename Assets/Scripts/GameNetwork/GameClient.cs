using ServerApp;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.Networking.Types;

namespace GameNetwork
{
	class GameClient
	{
		private int clientId = -1;
		private int roomId = -1;
		private TcpClient? client;
		private bool disconnected = false;
		private GameState lastState = null;
		private Thread pollTh;

		GameDataBuilder dataBuilder = new GameDataBuilder();

		public ConnectionResponse Connect(string host, int port, string userName, int modelId)
		{
			ConnectionResponse response = new ConnectionResponse()
			{
				success = false,
				playerId = -1
			};

			try
			{
				client = new TcpClient(host, port);

				// create connection request message
				ConnectionRequest connectMsg = new ConnectionRequest
				{
					userName = userName,
					modelId = modelId
				};

				// serialize the message
				string msg = connectMsg.Serialize();

				// send the message and receive response
				string responseMsg = SendMessage(msg);

				// deserialize received message
				response = dataBuilder.DeserializeConnectionResponse(responseMsg);

				if(response.success)
				{
					roomId = response.roomId;
					clientId = response.playerId;
					pollTh = new Thread(PollForMessages);
					pollTh.Start();
				}

			}
			catch (SocketException e)
			{
				Console.WriteLine("Client - SocketException: ", e);
				Disconnect();
			}

			return response;
		}

		public void Disconnect()
		{
			if (client != null)
			{
				client.Dispose();
			}
			disconnected = true;
		}

		public bool isDisconnected()
		{
			return disconnected;
		}

		public int GetClientId()
		{
			return clientId;
		}

		public int GetRoomId()
		{
			return roomId;
		}

		public GameState GetLastGameState()
		{
			return lastState;
		}

		public void SendMove(int boxId, int boxColor)
		{
			// create move request message
			MoveRequest moveMsg = new MoveRequest
			{
				playerId = clientId,
				boxId = boxId,
				boxColor = boxColor
			};

			// serialize the message
			string msg = moveMsg.Serialize();

			// send the message
			SendMessageAsync(msg);
		}

		/// <summary>
		/// Sends a message to server and waits for response
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private string SendMessage(string message)
		{
			NetworkStream stream = client.GetStream();
			string responseMsg = "";

			try
			{
				// translate the passed message into ASCII and store it as a Byte array.
				Byte[] data = Encoding.ASCII.GetBytes(message);

				// send the message
				stream.Write(data, 0, data.Length);

				// read the response
				data = new Byte[256];
				Int32 bytes = stream.Read(data, 0, data.Length);
				responseMsg = Encoding.ASCII.GetString(data, 0, bytes);
			}
			catch (SocketException e)
			{
				Console.WriteLine("Client - SocketException: ", e);
				Disconnect();
			}

			return responseMsg;
		}

		private void SendMessageAsync(string message)
		{
			NetworkStream stream = client.GetStream();
			try
			{
				// translate the passed message into ASCII and store it as a Byte array.
				Byte[] data = Encoding.ASCII.GetBytes(message);

				// send the message
				stream.Write(data, 0, data.Length);
			}
			catch (SocketException e)
			{
				Console.WriteLine("Client - SocketException: ", e);
				Disconnect();
			}
		}

		private void PollForMessages()
		{
			NetworkStream stream = client.GetStream();

			try
			{
				while (!disconnected)
				{
					// get new messages from the server
					if (stream.DataAvailable)
					{
						byte[] data = new Byte[256];
						Int32 bytes = stream.Read(data, 0, data.Length);
						string responseMsg = Encoding.ASCII.GetString(data, 0, bytes);

						if (responseMsg.Contains("*serverdisconnected*"))
						{
							Disconnect();
							return;
						}
						else if(responseMsg.Contains("GameStateMessage"))
						{
							// deserialize received message
							GameState state = dataBuilder.DeserializeGameState(responseMsg);
							lastState = state;
						}
					}

					Thread.Sleep(100);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Client - SocketException: ", e);
				Disconnect();
			}
		}
	}
}
