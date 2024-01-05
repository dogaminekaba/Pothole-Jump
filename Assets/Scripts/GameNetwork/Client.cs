using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameNetwork
{
	class Client
	{
		private int clientId = -1;
		TcpClient? client;

		public void Connect(string host, int port)
		{
			try
			{
				client = new TcpClient(host, port);

				// Translate the passed message into ASCII and store it as a Byte array.
				Byte[] data = System.Text.Encoding.ASCII.GetBytes("Hello");

				// Get a client stream for reading and writing.
				NetworkStream stream = client.GetStream();

				// Send the message to the connected TcpServer.
				stream.Write(data, 0, data.Length);

				// Receive the server response.

				// Buffer to store the response bytes.
				data = new Byte[256];

				// Read the first batch of the TcpServer response bytes.
				Int32 bytes = stream.Read(data, 0, data.Length);
				String responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
				Console.WriteLine("Client - Received: {0}", responseData);

				clientId = Convert.ToInt32(responseData);

			}
			catch (ArgumentNullException e)
			{
				Console.WriteLine("Client - ArgumentNullException: {0}", e);
			}
			catch (SocketException e)
			{
				Console.WriteLine("Client - SocketException: {0}", e);
			}
		}


		public int GetClientId()
		{
			return clientId;
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

				Console.WriteLine("Client - Sent: {0}", message);

				// Receive the server response.

				// Buffer to store the response bytes.
				data = new Byte[256];

				// Read the first batch of the TcpServer response bytes.
				Int32 bytes = stream.Read(data, 0, data.Length);
				responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
				Console.WriteLine("Client - Received: {0}", responseData);
			}
			catch (ArgumentNullException e)
			{
				Console.WriteLine("Client - ArgumentNullException: {0}", e);
			}
			catch (SocketException e)
			{
				Console.WriteLine("Client - SocketException: {0}", e);
			}

			return responseData;
		}
	}
}
