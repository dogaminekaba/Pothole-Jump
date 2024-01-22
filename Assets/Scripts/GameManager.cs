using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Assets.Scripts.GameData;

public class GameManager : MonoBehaviour
{

	public static readonly int boardSize = 6;
	private GameState gameState;

	// Start is called before the first frame update
	void Start()
	{
		gameState = GameState.MainMenu;

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void StartGame()
	{
		SceneManager.LoadScene("Gameplay");
	}
}
