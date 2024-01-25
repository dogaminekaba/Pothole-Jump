using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameNetwork;
using static Assets.Scripts.GameData;
using Unity.VisualScripting;

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
	public TextMeshProUGUI titleText;
	public TextMeshProUGUI infoText;
	public TextMeshProUGUI infoTextTop;
	public TextMeshProUGUI Player1Info;
	public TextMeshProUGUI Player2Info;

	// Game elements
	GameManagerState mngState;
	public BoardController boardMng;
	public static readonly int boardSize = 6;
	public GameObject boxPrefab;
	public GameObject startBoxPrefab;
	public static bool canPlay = false;
	public static bool playedTurn = false;

	private List<int> solidBoxIds = new List<int>();
	private Dictionary<int, GameObject> boxDict;
	private static int currentBoxId;
	private static int currentBoxColor;
	private int lastPlayerBoxId;
	private int lastOpponentBoxId;

	// Network
	private NetworkManager networkMng;
	private GameState gameState;
	private int clientId = -1;

	// Characters
	public GameObject playerChar;
	private GameObject opponentChar;
	private PlayerSelector playerSelector;
	private int modelId = 0;
	private bool opponentCreated = false;
	private Player playerInfo;
	private Player opponentInfo;

	private int solidBoxesFound = 0;

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

		playerSelector = playerChar.GetComponent<PlayerSelector>();
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

		if (mngState == GameManagerState.WaitingOthers)
		{
			gameState = networkMng.GetLastGameState();
			if (gameState != null)
			{
				solidBoxIds = gameState.solidBoxIds;

				if (gameState.playerList.Count > 1)
					mngState = GameManagerState.PlayerJoined;
			}
		}

		if (mngState == GameManagerState.PlayerJoined)
		{
			foreach (var player in gameState.playerList)
			{
				if(player.id == clientId)
				{
					playerInfo = player;
				}
				else
				{
					opponentInfo = player;
					lastOpponentBoxId = opponentInfo.currentBoxId;
					// show other character
					int opponentModelId = player.modelId;
					Vector3 pos = new Vector3(-4f, 0f, 5f);
					opponentChar = playerSelector.InstantiateCharacter(opponentModelId, pos, playerChar.transform.rotation);
					opponentChar.gameObject.SetActive(true);
				}
			}

			mngState = GameManagerState.Ready;
		}

		if (mngState == GameManagerState.Ready)
		{
			infoText.text = opponentInfo.userName + " is joined! Click \"play!\" to start the game!";

			if (!playBtn.gameObject.activeSelf)
				playBtn.gameObject.SetActive(true);
		}

		if (mngState == GameManagerState.Playing)
		{
			// is it my turn
			gameState = networkMng.GetLastGameState();
			
			if (gameState != null)
			{
				canPlay = gameState.turnPlayerId == clientId;

				foreach (var player in gameState.playerList)
				{
					if (player.id == clientId)
					{
						playerInfo = player;

					}
					else
					{
						opponentInfo = player;
					}
				}

				if(opponentInfo.currentBoxId != lastOpponentBoxId)
				{
					lastOpponentBoxId = opponentInfo.currentBoxId;
					if (lastOpponentBoxId > 0)
					{
						boxDict[lastOpponentBoxId].GetComponent<BoxController>().RevealColor();
						if (solidBoxIds.Contains(lastOpponentBoxId))
							++solidBoxesFound;

					}
				}

				if (playerInfo.currentBoxId != lastPlayerBoxId)
				{
					lastPlayerBoxId = playerInfo.currentBoxId;
					if (lastPlayerBoxId > 0 && solidBoxIds.Contains(lastPlayerBoxId))
					{
						++solidBoxesFound;
					}
				}
			}

			if(playedTurn)
			{
				networkMng.SendMove(currentBoxId, currentBoxColor);
				playedTurn = false;
			}

			updateUiElements();
		}
	}

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}

	private void OnDestroy()
	{
		networkMng?.DisconnectClient();
	}

	public void StartGame()
	{
		SceneManager.LoadScene("Gameplay");

		boxDict = boardMng.InitializeBoard(boardSize, solidBoxIds, startBoxPrefab, boxPrefab);

		// create player
		Vector3 pos = boardMng.startBox1.transform.position + new Vector3(0, boardMng.boxSize, 0);
		playerChar.transform.position = pos;

		pos = boardMng.startBox2.transform.position + new Vector3(0, boardMng.boxSize, 0);
		opponentChar = playerSelector.InstantiateCharacter(2, pos, playerChar.transform.rotation);
		opponentChar.transform.Rotate(Vector3.up, 180);
		opponentChar.gameObject.SetActive(true);

		infoText.gameObject.SetActive(false);
		titleText.gameObject.SetActive(false);
		playBtn.gameObject.SetActive(false);

		infoTextTop.gameObject.SetActive(true);
		Player1Info.gameObject.SetActive(true);
		Player2Info.gameObject.SetActive(true);

		mngState = GameManagerState.Playing;
	}

	public void updateUiElements()
	{
		Player1Info.text = playerInfo.userName + ": " + playerInfo.score;
		Player2Info.text = opponentInfo.userName + ": " + opponentInfo.score;

		// game over
		if (solidBoxesFound == solidBoxIds.Count)
		{
			if(playerInfo.score == opponentInfo.score)
			{
				infoTextTop.text = "Wow... It's... A tie... ";
			}
			else
			{
				string winnerName = playerInfo.score > opponentInfo.score ? playerInfo.userName : opponentInfo.userName;
				infoTextTop.text = "All solid boxes are found! " + winnerName + " WON!";
			}

			canPlay = false;
			mngState = GameManagerState.GameOver;
		}
		else
		{
			if (canPlay)
			{
				infoTextTop.text = "It's " + playerInfo.userName + "'s turn.";
			}
			else
			{
				infoTextTop.text = "It's " + opponentInfo.userName + "'s turn.";
			}
		}
	}

	public static void UpdateCurrentBox(int boxId, int boxColor)
	{
		currentBoxId = boxId;
		currentBoxColor = boxColor;

		playedTurn = true;
	}

	// Network Related Methods

	public void ConnectServer()
	{
		if (infoText != null)
		{
			infoText.text = "Connecting to server...";
		}

		string username = usernameField?.text;
		modelId = PlayerSelector.getPlayerModelId();

		ConnectionResponse response = networkMng.ConnectToGameServer(username, modelId);

		if (response.success)
		{
			clientId = response.playerId;

			infoText.text = username + " is connected." + ". Waiting for player 2 to join...";

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
