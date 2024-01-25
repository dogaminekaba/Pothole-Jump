using System.Net.Sockets;
using System.Net;
using ServerApp;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Diagnostics;


namespace GameNetwork
{
	class GameServer
	{
		private int maxRoomCount = 10;
		private int maxClientCount = 20; // a room can hold 2 players max

		private bool stopListening = false;
		private TcpListener server = null;
		private Thread listenerThread = null;
		private List<Thread> clientThreads = null;

		private Stack<int> availablePlayerIds = null;
		private Stack<int> availableRoomIds = null;
		private HashSet<int> waitingRoomIds = null;
		private List<TcpClient> connectedClients = null;

		// keep a record of the last states and clients in the rooms for broadcasting
		private Dictionary<int, GameState> roomGameStates = new Dictionary<int, GameState>();

		public GameServer()
		{
			
		}

		public void StartServer(string host, int port)
		{
			InitializeServer();

			try
			{
				// set up server
				IPAddress localAddr = IPAddress.Parse(host);
				server = new TcpListener(localAddr, port);
				server.Start();

				// listen to client connections
				listenerThread = new Thread(listenForConnections);
				listenerThread.Start();

			}
			catch (SocketException e)
			{
				Console.WriteLine("Start() - SocketException: " + e);
				StopServer();
			}
		}

		private void InitializeServer()
		{
			waitingRoomIds = new HashSet<int>();
			// Create a stack of room Ids
			availableRoomIds = new Stack<int>();
			for (int i = maxRoomCount; i > 0; --i)
			{
				availableRoomIds.Push(100 + i);
			}

			// Create a stack of player Ids
			availablePlayerIds = new Stack<int>();
			for (int i = maxClientCount; i > 0; --i)
			{
				availablePlayerIds.Push(i);
			}

			connectedClients = new List<TcpClient>();
			clientThreads = new List<Thread>();
		}

		public void StopServer()
		{
			stopListening = true;

			try
			{
				if (server != null)
				{
					foreach (TcpClient client in connectedClients)
					{
						SendMessage(client.GetStream(), "*serverdisconnected*");
						client.Close();
					}
					server.Stop();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Server - Exception: " + e);
			}
		}

		private void listenForConnections()
		{
			Console.WriteLine("Server - listenForConnections ");

			while (!stopListening)
			{
				// Accept connection requests if we have enough space
				if (availablePlayerIds.Count > 0)
				{
					Console.WriteLine("Server - Waiting for a connection...");

					try
					{
						TcpClient client = server.AcceptTcpClient();

						lock(connectedClients)
						{
							connectedClients.Add(client);
						}

						Thread clientTh = new Thread(() => listenClient(client));

						lock (clientThreads)
						{
							clientThreads.Add(clientTh);
						}

						clientTh.Start();

						Console.WriteLine("Server - Connected!");
					}
					catch (SocketException e)
					{
						if ((e.SocketErrorCode == SocketError.Interrupted))
							Console.WriteLine("Socket connection is interrupted.");
						else
							Console.WriteLine("Server - Exception: " + e);

						stopListening = true;
					}

				}
				else
				{
					// Reject client
					Console.WriteLine("Server is full!");
				}
			}
		}

		private void listenClient(TcpClient client)
		{
			GameDataBuilder builder = new GameDataBuilder();
			bool isListening = true;
			int clientId = -1;
			int roomId = -1;
			bool clientDisonnected = false;

			Stopwatch sw = new Stopwatch();
			sw.Start();

			try
			{
				// buffer for reading data
				Byte[] data = new Byte[256];

				// get a stream object for reading and writing
				NetworkStream stream = client.GetStream();

				int bytes = 0;

				// receive all the data sent by the client.
				while (isListening)
				{
					if (stream.DataAvailable)
					{
						data = new Byte[256];
						bytes = stream.Read(data, 0, data.Length);
						string requestData = Encoding.ASCII.GetString(data, 0, bytes);

						// Connection
						if (requestData.Contains("ConnectionRequest"))
						{
							ConnectionRequest? ConReqMsg = builder.DeserializeMsg<ConnectionRequest>(requestData);
							ConnectionResponse response;

							if (ConReqMsg != null)
							{
								response = HandleConnectionRequest(ConReqMsg, builder);
								string responseMsg = builder.SerializeMsg(response);
								SendMessage(stream, responseMsg);

								clientId = response.playerId;
								roomId = response.roomId;
								isListening = response.success;
							}
						}

						// GameState
						if (requestData.Contains("GameStateRequest"))
						{
							GameStateRequest? stateReqMsg = builder.DeserializeMsg<GameStateRequest>(requestData);
							GameState state;

							if (stateReqMsg != null)
							{
								lock (roomGameStates)
								{
									state = roomGameStates[stateReqMsg.roomId];
								}

								string responseMsg = builder.SerializeMsg(state);
								SendMessage(stream, responseMsg);
							}
						}

						// Move

						// reset timer for disconnection detection
						sw.Restart();
					}
					else
					{
						if (sw.ElapsedMilliseconds > 1000) throw new TimeoutException();
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("listenClient - Exception: " + e);
				clientDisonnected = true;
			}

			Console.WriteLine("Client " + clientId + " disconnected.");

			// release resources
			
			if(clientDisonnected) {
				lock (availablePlayerIds)
				{
					availablePlayerIds.Push(clientId);
				}

				int roomPlayerCount = 0;

				lock (roomGameStates)
				{
					roomGameStates[roomId].playerList.RemoveAll(p => p.id == clientId);
					roomPlayerCount = roomGameStates[roomId].playerList.Count;

					if (roomPlayerCount < 1)
					{
						roomGameStates.Remove(roomId);
					}
				}

				if (roomPlayerCount < 1)
				{
					lock(waitingRoomIds)
					{
						waitingRoomIds.Remove(roomId);
					}
					lock (availableRoomIds)
					{
						availableRoomIds.Push(roomId);
					}
				}
			}
			
		}

		private ConnectionResponse HandleConnectionRequest(ConnectionRequest requestMsg, GameDataBuilder builder)
		{
			int playerId = -1;
			int modelId = requestMsg.modelId;
			string username = requestMsg.userName;
			int roomId = -1;
			bool createRoom = false;
			bool createRoomSuccessful = false;

			ConnectionResponse response = new ConnectionResponse()
			{
				success = false,
				roomId = roomId,
				playerId = playerId,
			};

			// create a unique id
			lock (availablePlayerIds)
			{
				if (availablePlayerIds.Count > 0)
				{
					playerId = availablePlayerIds.Pop();
				}
				else
				{
					return response;
				}
			}

			lock (waitingRoomIds)
			{
				createRoom = waitingRoomIds.Count == 0;
			}

			// create a new room
			if (createRoom)
			{
				lock (availableRoomIds)
				{
					createRoomSuccessful = availableRoomIds.TryPop(out roomId);
				}

				if (createRoomSuccessful)
				{
					GameState state = builder.CreateGameState(roomId, 6);

					Player player = new Player()
					{
						id = playerId,
						modelId = modelId,
						userName = username,
						posX = 0,
						posY = 0,
						score = 0
					};

					state.playerList.Add(player);

					lock (roomGameStates)
					{
						roomGameStates.Add(roomId, state);
					}

					lock (waitingRoomIds)
					{
						waitingRoomIds.Add(roomId);
					}
				}
				else
				{
					lock (availablePlayerIds)
					{
						availablePlayerIds.Push(playerId);
					}
					return response;
				}
			}
			// join a waiting room
			else
			{
				Player player = new Player()
				{
					id = playerId,
					modelId = modelId,
					userName = username,
					posX = 0,
					posY = 0,
					score = 0
				};

				lock (roomGameStates)
				{
					roomGameStates[roomId].playerList.Add(player);
				}
			}

			response = new ConnectionResponse()
			{
				playerId = playerId,
				roomId = roomId,
				success = true
			};

			return response;
		}

		/// <summary>
		/// Sends a message to client
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private void SendMessage(NetworkStream stream, string message)
		{
			try
			{
				// create a byte array
				byte[] data = Encoding.ASCII.GetBytes(message);

				// Send back a response.
				stream.Write(data, 0, data.Length);
				Console.WriteLine("Server - Sent: " + message);
			}
			catch (ArgumentNullException e)
			{
				Console.WriteLine("Server - ArgumentNullException: ", e);
			}
			catch (SocketException e)
			{
				Console.WriteLine("Server - SocketException: ", e);
			}

		}
	}
}
