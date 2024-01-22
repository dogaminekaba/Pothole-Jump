using System.Net.Sockets;
using System.Net;
using ServerApp;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;


namespace GameNetwork
{
	class GameServer
	{

		private int maxRoomCount = 10;
		private int maxClientCount = 40; // A room can hold 4 players max

		private bool stopListening = false;
		private TcpListener server = null;
		private Thread listenerThread = null;
		private List<Thread> clientThreads = null;

		private Stack<int> availablePlayerIds = null;
		private Stack<int> availableRoomIds = null;
		private List<TcpClient> clientList = null;

		public GameServer()
		{
			InitializeServer();
		}

		public void Start(string host, int port)
		{
			try
			{
				// set up server
				IPAddress localAddr = IPAddress.Parse(host);
				server = new TcpListener(localAddr, port);

				// Start listening for client requests.
				server.Start();

				// Run the thread for listening client connections
				listenerThread = new Thread(listenForConnections);
				listenerThread.Start();

			}
			catch (SocketException e)
			{
				Console.WriteLine("Start() - SocketException: " + e);
				StopServer();
			}
		}

		public void StopServer()
		{
			stopListening = true;

			// TODO - close client connections
			// TODO - make sure threads are not running
			try
			{
				if (server != null)
				{
					foreach (TcpClient client in clientList)
					{
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

		private void InitializeServer()
		{
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

			clientList = new List<TcpClient>();
			clientThreads = new List<Thread>();
		}

		private void listenForConnections()
		{
			Console.WriteLine("Server - listenForConnections ");

			while (!stopListening)
			{
				// Accept connection requests if we have enough space
				if (availablePlayerIds.TryPop(out int id))
				{
					Console.WriteLine("Server - Waiting for a connection... ");

					try
					{
						TcpClient client = server.AcceptTcpClient();
						clientList.Add(client);

						// TODO - room number
						Thread clientTh = new Thread(() => listenClient(client, id));
						clientThreads.Add(clientTh);
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
			}
		}

		private void listenClient(TcpClient client, int clientId)
		{
			
			GameDataBuilder builder = new GameDataBuilder();
			bool setupClient = true;
			int roomId = 123;

			// Buffer for reading data
			Byte[] data = new Byte[256];

			// Get a stream object for reading and writing
			NetworkStream stream = client.GetStream();

			int bytes = 0;

			// Loop to receive all the data sent by the client.
			while (!stopListening)
			{
				try
				{
					if ((bytes = stream.Read(data, 0, data.Length)) != 0)
					{
						// Send client data
						if (setupClient)
						{
							string requestData = Encoding.ASCII.GetString(data, 0, bytes);
							ConnectionRequest? requestMsg = builder.DeserializeMsg<ConnectionRequest>(requestData);

							Console.WriteLine("Server - Received: ", requestMsg);

							//request = builder.deserializeConnectionRequest(receivedMsg);
							//Console.WriteLine("Server - Received: " + request);
							Console.WriteLine("Server - setupClient");

							// create response
							ConnectionResponse responseMsg = new ConnectionResponse()
							{
								playerId = clientId,
								roomId = roomId
							};

							// Serialize the message and create a byte array
							string responseData = builder.SerializeMsg(responseMsg);
							data = Encoding.ASCII.GetBytes(responseData);

							// Send back a response.
							stream.Write(data, 0, data.Length);
							Console.WriteLine("Server - Sent: " + responseMsg);

							setupClient = false;
						}
						else
						{
							// TODO
							// calculate new state and broadcast
						}

					}
				}
				catch (Exception e)
				{
					Console.WriteLine("listenClient - Exception: " + e);
				}

			}

			// TODO - release sources like id
		}
	}
}
