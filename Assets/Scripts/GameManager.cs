using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.GameData;

public class GameManager : MonoBehaviour
{

	public static readonly int boardSize = 2;
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
