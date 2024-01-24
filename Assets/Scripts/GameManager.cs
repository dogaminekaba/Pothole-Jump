using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Assets.Scripts.GameData;

public class GameManager : MonoBehaviour
{
	public BoardController boardMng;
	public static readonly int boardSize = 5;
	private GameState gameState;
	List<int> solidBoxIds = new List<int>();

	public GameObject boxPrefab;
	public GameObject startBoxPrefab;
	List<BoxController> boxList = new List<BoxController>();

	// Start is called before the first frame update
	void Start()
	{
		gameState = GameState.MainMenu;

		// TODO remove
		InitSolidBoxList();
	}

	// Update is called once per frame
	void Update()
	{

	}

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
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
}
