using UnityEngine;
using UnityEngine.UI;
using GameNetwork;
using TMPro;

public class NetworkManager : MonoBehaviour
{
	public Button startServerBtn;
	public Button connectBtn;
	public TextMeshProUGUI infoText;

	private GameClient myClient;

	private bool isMyTurn = false;

	private string serverIp = "127.0.0.1";
	private int serverPort = 13000;

	// Start is called before the first frame update
	void Start()
	{
		myClient = new GameClient();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void CreateRoom()
	{
		//ConnectToGameServer();

		// TODO - Show the room number
	}

	public void JoinRoom()
	{
		// TODO - Show the input UI to enter Room number

		// ConnectToGameServer();
	}

	public void ConnectServer()
	{
		ConnectToGameServer();
	}

	private int ConnectToGameServer()
	{
		int clientId = -1;

		// TODO - Check if connected before

		infoText.text = "Connecting the server...";
		int result = myClient.Connect(serverIp, serverPort);

        if (result == 0)
        {
			clientId = myClient.GetClientId();
			int roomId = myClient.GetRoomId();

			infoText.text = clientId + " is connected. Waiting for friends in room: " + roomId;
		}
		else
		{
			infoText.text = "Cannot connect to server. Try Again.";
		}

		return clientId;
	}


}
