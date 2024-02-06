using System.Net.Sockets;
using System.Net;
using ServerApp;
using System.Text;
using System.Diagnostics;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;


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

		// keep a record of the last states and clients in the rooms for broadcasting
		private Dictionary<int, List<TcpClient>> connectedClients = null;
		private Dictionary<int, GameState> roomGameStates = null;

		private int boardSize = 6;

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

			connectedClients = new Dictionary<int, List<TcpClient>>();
			clientThreads = new List<Thread>();
			roomGameStates = new Dictionary<int, GameState>();
		}

		public void StopServer()
		{
			stopListening = true;

			try
			{
				if (server != null)
				{
					lock (connectedClients)
					{
						foreach (KeyValuePair<int, List<TcpClient>> entry in connectedClients)
						{
							foreach (TcpClient client in entry.Value)
							{
								NetworkStream stream = client.GetStream();
								SendMessage(stream, "ServerDisconnected");
								stream.Flush();
								stream.Close();
								client.Close();
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Server - Exception: " + e);
			}
			server?.Stop();
		}

		private void listenForConnections()
		{
			Console.WriteLine("Server - listenForConnections ");
			Console.WriteLine("Server - Waiting for a connection...");

			while (!stopListening)
			{
				// Accept connection requests if we have enough space
				if (availablePlayerIds.Count > 0)
				{
					try
					{
						TcpClient client = server.AcceptTcpClient();

						Thread clientTh = new Thread(() => ListenClient(client));

						lock (clientThreads)
						{
							clientThreads.Add(clientTh);
						}

						clientTh.Start();
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

		private void ListenClient(TcpClient client)
		{
			GameDataBuilder builder = new GameDataBuilder();
			bool isListening = true;
			int clientId = -1;
			int roomId = -1;
			bool clientDisonnected = false;
			string userName = "";

			try
			{
				
				// get a stream object for reading and writing
				NetworkStream stream = client.GetStream();
				List<TcpClient> players = new List<TcpClient>();

				// timer to track client connection
				Stopwatch sw = new Stopwatch();
				sw.Start();

				// receive all the data sent by the client.
				while (isListening)
				{
					if (stream.DataAvailable)
					{
						string requestData = ReadReceivedData(stream);
						// Connection
						if (requestData.Contains("ConnectionRequestMessage"))
						{
							ConnectionRequest? ConReqMsg = builder.DeserializeConnectionRequest(requestData);
							ConnectionResponse response;

							if (ConReqMsg != null)
							{
								response = HandleConnectionRequest(ConReqMsg, builder);
								string responseMsg = response.Serialize();
								SendMessage(stream, responseMsg);

								clientId = response.playerId;
								roomId = response.roomId;
								isListening = response.success;

								// update
								if(isListening)
								{
									lock (connectedClients)
									{
										if(connectedClients.TryGetValue(roomId, out players))
										{
											players.Add(client);
										}
										else
										{
											players = [client];
											connectedClients.Add(roomId, players);
										}
									}

									userName = ConReqMsg.userName;
									Console.WriteLine("Client " + clientId + ": " + userName + " is connected.");
								}
							}
						}

						// Move
						else if (requestData.Contains("MoveRequestMessage"))
						{
							MoveRequest? MoveReqMsg = builder.DeserializeMoveRequest(requestData);
							GameState lastState  = HandleMoveRequest(MoveReqMsg, builder, roomId);
						}

						// ClientPulse
						else if (requestData.Contains("ClientPulse"))
						{
							GameState lastState;
							lock (roomGameStates)
							{
								lastState = roomGameStates[roomId];
							}
							string responseMsg = lastState.Serialize();
							SendMessage(stream, responseMsg);
						}

						// reset timer for disconnection detection
						sw.Restart();
					}
					else
					{
						if (sw.ElapsedMilliseconds > 500) throw new TimeoutException();
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("listenClient - Exception: " + e);
				clientDisonnected = true;
			}

			Console.WriteLine("Client " + clientId + ": " + userName + " is disconnected.");


			if (clientDisonnected) {
				int roomPlayerCount = 0;

				// disconnect other clients as well
				List<TcpClient> players;
				lock (connectedClients)
				{
					if (connectedClients.TryGetValue(roomId, out players))
					{
						players.Add(client);
					}
					else
					{
						players = [client];
						connectedClients.Add(roomId, players);
					}
				}

				// broadcast
				string broadcastMsg = "ServerDisconnected";
				foreach (TcpClient player in players)
				{
					if (player.Connected)
					{
						NetworkStream playerStream = player.GetStream();

						if(playerStream != null && playerStream.CanWrite)
						{
							SendMessage(playerStream, broadcastMsg);
							playerStream.Close();
						}

						player?.Close();
					}
				}

				// release resources
				lock (roomGameStates)
				{
					if (roomId > -1 && roomGameStates.ContainsKey(roomId))
					{
						roomGameStates[roomId].playerList.RemoveAll(p => p.id == clientId);
						roomPlayerCount = roomGameStates[roomId].playerList.Count;

						// remove the room game state
						roomGameStates.Remove(roomId);
					}
				}

				if (roomPlayerCount == 0)
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

				lock (availablePlayerIds)
				{
					availablePlayerIds.Push(clientId);
				}
			}
		}

		private string ReadReceivedData(NetworkStream stream)
		{
			string requestData = "";

			// buffer for reading data
			Byte[] data = new Byte[256];
			int bytes = stream.Read(data, 0, data.Length);

			if (bytes > 0)
			{
				requestData = Encoding.ASCII.GetString(data, 0, bytes);
			}

			return requestData;
		}

		private GameState HandleMoveRequest(MoveRequest moveReqMsg, GameDataBuilder builder, int roomId)
		{
			GameState state;

			lock (roomGameStates)
			{
				state = roomGameStates[roomId];
			}

			foreach(Player player in state.playerList)
			{
				if (player.id == moveReqMsg.playerId)
				{
					player.currentBoxId = moveReqMsg.boxId;

					if (moveReqMsg.boxColor == 0) // solid box
					{
						player.score += 10;
					}
				}
				else
				{
					state.turnPlayerId = player.id;
				}
			}

			return state;
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
				if (!createRoom)
				{
					roomId = waitingRoomIds.First();
					waitingRoomIds.Remove(roomId);
				}
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
					GameState state = builder.CreateGameState(roomId, boardSize);

					state = builder.AddPlayer(state, playerId, modelId, username);

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
				lock (roomGameStates)
				{
					GameState state = roomGameStates[roomId];
					roomGameStates[roomId] = builder.AddPlayer(state, playerId, modelId, username);
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

				// Send message
				stream.Write(data, 0, data.Length);
				stream.Flush();
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
