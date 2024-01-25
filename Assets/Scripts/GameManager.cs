using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameNetwork;
using static Assets.Scripts.GameData;

public class GameManager : MonoBehaviour
{
	// UI Related Field
	public TMP_InputField usernameField;
	public Button connectBtn;
	public Button prevBtn;
	public Button nextBtn;
	public Button createRoomBtn;
	public TMP_InputField roomIdField;
	public Button joinRoomBtn;
	public Button playBtn;
	public TextMeshProUGUI infoText;

	// Game elements
	GameManagerState mngState;
	public BoardController boardMng;
	public static readonly int boardSize = 5;
	public GameObject boxPrefab;
	public GameObject startBoxPrefab;

	// Network
	private NetworkManager networkMng;
	private GameState gameState;
	private int clientId = -1;

	// Characters
	public GameObject player;
	private PlayerSelector playerSelector;
	private int modelId = 0;

	private List<int> solidBoxIds = new List<int>();
	private static GameObject currentBox;
	private static int currentBoxId;
	private GameObject opponentChar;

	int score = 0;

	// Start is called before the first frame update
	void Start()
	{
		mngState = GameManagerState.StartMenu;

		// setup Network Manager
		networkMng = new NetworkManager();

		// setup UI
		if (usernameField != null && roomIdField != null)
		{
			usernameField.characterLimit = 10;
			roomIdField.characterLimit = 3;
		}

		if (infoText != null)
		{
			infoText.text = "Select your character before connecting!";
		}

		playerSelector = player.GetComponent<PlayerSelector>();
	}

	// Update is called once per frame
	void Update()
	{
		if (networkMng.isDisconnected() && mngState != GameManagerState.StartMenu)
		{
			clientId = -1;

			usernameField.gameObject.SetActive(true);
			connectBtn.gameObject.SetActive(true);
			prevBtn.gameObject.SetActive(true);
			nextBtn.gameObject.SetActive(true);

			infoText.text = "Something went wrong :( Please try again!";
			mngState = GameManagerState.StartMenu;
		}
		
		if(mngState == GameManagerState.WaitingOthers)
		{
			gameState = networkMng.GetLastGameState();
			if (gameState != null)
				mngState = GameManagerState.Ready;
		}

		if(mngState == GameManagerState.Ready)
		{
			List<Player> playerList = gameState.playerList;
			foreach (Player player in playerList)
			{
				if(player.id != clientId)
				{
					// show other character
					int opponentModelId = player.modelId;
					Vector3 pos = new Vector3(5f, 0.7f, -3f);
					opponentChar = playerSelector.InstantiateCharacter(opponentModelId, pos);
				}
			}

		}
	}

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}

	private void OnDestroy()
	{
		networkMng.DisconnectClient();
	}

	public void StartGame()
	{
		SceneManager.LoadScene("Gameplay");

		InitSolidBoxList();
	}

	private void InitSolidBoxList()
	{
		int startIndex = 0;
		int endIndex = boardSize;

		for (int i = 0; i < boardSize; i++)
		{
			int median = (startIndex + endIndex) / 2;

			int solid1 = Random.Range(startIndex, median) + 1;
			int solid2 = Random.Range(median, endIndex) + 1;

			solidBoxIds.Add(solid1);
			solidBoxIds.Add(solid2);

			startIndex += boardSize;
			endIndex += boardSize;
		}

		boardMng.InitializeBoard(boardSize, solidBoxIds, startBoxPrefab, boxPrefab);
	}

	public static void UpdateCurrentBox(int boxId, GameObject box, bool isSolid)
	{
		currentBoxId = boxId;
		currentBox = box;
	}

	// Network Related Methods

	public void ConnectServer()
	{
		if (infoText != null)
		{
			infoText.text = "Connecting the server...";
		}

		string username = usernameField?.text;
		modelId = PlayerSelector.getPlayerModelId();

		ConnectionResponse response = networkMng.ConnectToGameServer(username, modelId);

		if (response.success)
		{
			clientId = response.playerId;

			infoText.text = username + " is connected. id:" + clientId + ". Waiting for player 2 to join...";

			usernameField.gameObject.SetActive(false);
			connectBtn.gameObject.SetActive(false);

			prevBtn.gameObject.SetActive(false);
			nextBtn.gameObject.SetActive(false);

			mngState = GameManagerState.WaitingOthers;

			//// Room setup
			//createRoomBtn.gameObject.SetActive(true);
			//roomIdField.gameObject.SetActive(true);
			//joinRoomBtn.gameObject.SetActive(true);
		}
		else
		{
			infoText.text = "Cannot connect to server. Try Again.";
		}
	}

	public void SendMove()
	{
		// TODO - Show the input UI to enter Room number
	}




}
