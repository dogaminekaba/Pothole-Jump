using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	enum GameState
	{
		MainMenu,
		MatchMaking,
		Waiting,
		YourTurn,
		GameOver
	}

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
}
