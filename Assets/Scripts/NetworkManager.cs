using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameNetwork;
using TMPro;

public class NetworkManager : MonoBehaviour
{
	public Button startServerBtn;
	public Button connectBtn;
	public TextMeshProUGUI infoText;

	private Server gameServer;
	private Client myClient;

	private string serverIp = "127.0.0.1";
	private int serverPort = 13000;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void StartServer()
	{
		// Create the game server
		gameServer = new Server();

		infoText.text = "Server is starting";

		gameServer.Start(serverIp, serverPort);

		infoText.text = "Server is running.";
	}

	public void ConnectToServer()
	{
		myClient = new Client();

		myClient.Connect(serverIp, serverPort);
	}


}
