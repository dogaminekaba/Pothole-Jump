using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;
using System.Threading;
using UnityEngine.XR;
using System.IO;
using UnityEditor.PackageManager;
using System.Collections.Generic;

namespace GameNetwork
{
	class Server
	{
		int maxClientCount = 10;

		private bool quitGame = false;
		private TcpListener server = null;
		private Thread listenerThread = null;
		private List<Thread> clientThreads = null;

		private Stack<int> availableIds = null;
		private List<TcpClient> clientList = null;

		public Server()
		{
			availableIds = new Stack<int>();

			for (int i = maxClientCount; i > 0; --i) { 
				availableIds.Push(i);
			}

			clientList = new List<TcpClient>();
			clientThreads = new List<Thread>();
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
				Debug.Log("Server - SocketException: {0}" + e);
				server.Stop();
			}
		}

		void StopServer()
		{

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
				Debug.Log("Server - Exception: {0}" + e);
			}

			quitGame = true;
		}

		private void listenForConnections()
		{
			Debug.Log("Server - listenForConnections ");
			while (!quitGame)
			{
				// Accept connection requests
				if (availableIds.TryPop(out int id))
				{
					Debug.Log("Server - Waiting for a connection... ");

					TcpClient client = server.AcceptTcpClient();
					clientList.Add(client);

					Thread clientTh = new Thread(() => listenClient(client, id));
					clientThreads.Add(clientTh);
					clientTh.Start();

					Debug.Log("Server - Connected!");
				}

			}
		}

		private void listenClient(TcpClient client, int assignedId)
		{
			bool setupClient = true;

			// TODO - Use serialized data instead of string
			String data = null;

			// Buffer for reading data
			Byte[] bytes = new Byte[256];


			// Get a stream object for reading and writing
			NetworkStream stream = client.GetStream();

			int i;

			// Loop to receive all the data sent by the client.
			while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
			{
				// Translate data bytes to a ASCII string.
				data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
				Debug.Log("Server - Received: {0}" + data);

				// Send client data
				if (setupClient)
				{
					data = "id:" + assignedId;
				}
				else
				{
					data = "Game Data";
				}

				byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

				// Send back a response.
				stream.Write(msg, 0, msg.Length);
				Debug.Log("Server - Sent: {0}" + data);
			}

			// TODO - release sources like id
		}
	}
}
