using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerNum = -1;

    // Start is called before the first frame update
    void Start()
    {
        int boardSize = GameManager.boardSize;
		float posX = (boardSize - 1) / 2f;

		if (playerNum == 0)
            transform.position = new Vector3(posX / 2, 1, -1);
		if (playerNum == 1)
			transform.position = new Vector3(posX / 2, 1, boardSize);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
