using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public GameObject box;
    public GameObject startBox;
    float boxSize = 0.95f;

    // Start is called before the first frame update
    void Start()
    {
        InitializeBoard(GameManager.boardSize);

	}

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitializeBoard(int boardSize)
    {
        float startBoxPosX = (boardSize - 1) / 2f;

		// create start platforms
		startBox.transform.localScale = new Vector3(boardSize, boxSize, boxSize);
		box.transform.localScale = new Vector3(boxSize, boxSize, boxSize);

		Vector3 pos = new Vector3(startBoxPosX, boxSize / 2, -1);
		Instantiate(startBox, pos, Quaternion.identity, gameObject.transform);

        pos = new Vector3(startBoxPosX, boxSize / 2, boardSize);
		Instantiate(startBox,pos, Quaternion.identity, gameObject.transform);

		// create game board
		for (int i = 0; i < boardSize; i++)
        {
            for(int j = 0; j < boardSize; j++)
            {
				pos = new Vector3(i, boxSize / 2, j);
				Instantiate(box, pos, Quaternion.identity, gameObject.transform);
            }
        }
    }
}
