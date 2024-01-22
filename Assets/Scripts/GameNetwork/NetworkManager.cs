using UnityEngine;
using UnityEngine.UI;
using GameNetwork;
using TMPro;

public class NetworkManager : MonoBehaviour
{
	public TMP_InputField usernameField;
	public Button connectBtn;
	public Button prevBtn;
	public Button nextBtn;
	public Button createRoomBtn;
	public TMP_InputField roomIdField;
	public Button joinRoomBtn;
	public Button playBtn;
	public TextMeshProUGUI infoText;

	private GameClient myClient;

	private bool isMyTurn = false;

	private string serverIp = "127.0.0.1";
	private int serverPort = 13000;

	// Start is called before the first frame update
	void Start()
	{
		myClient = new GameClient();
		usernameField.characterLimit = 10;
		roomIdField.characterLimit = 3;

		infoText.text = "Select your character before connecting!";
	}

	// Update is called once per frame
	void Update()
	{

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

		string userName = usernameField.text;

		int result = myClient.Connect(serverIp, serverPort, userName);

		if (result == 0)
		{
			clientId = myClient.GetClientId();
			int roomId = myClient.GetRoomId();

			infoText.text = userName + " is connected. id:" + clientId;

			usernameField.gameObject.SetActive(false);
			connectBtn.gameObject.SetActive(false);

			prevBtn.gameObject.SetActive(false);
			nextBtn.gameObject.SetActive(false);

			// Room setup
			createRoomBtn.gameObject.SetActive(true);
			roomIdField.gameObject.SetActive(true);
			joinRoomBtn.gameObject.SetActive(true);
		}
		else
		{
			infoText.text = "Cannot connect to server. Try Again.";
		}

		return clientId;
	}

	public void CreateRoom()
	{
		GameState responseMsg = myClient.RequestCreateRoom(2);

		infoText.text = "Room is created > " + responseMsg.roomId;

		// TODO - Show the room number
	}

	public void JoinRoom()
	{
		// TODO - Show the input UI to enter Room number
	}


}
