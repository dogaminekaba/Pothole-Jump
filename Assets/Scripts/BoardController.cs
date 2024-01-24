using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    float boxSize = 0.95f;

    // Start is called before the first frame update
    void Start()
    {
        
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public List <GameObject> InitializeBoard(int boardSize, List<int> solidBoxIds, GameObject startBoxPrefab, GameObject boxPrefab)
    {
		List <GameObject> boxList = new List <GameObject>();

		float startBoxPosX = (boardSize - 1) / 2f;

		// create starting platforms
		startBoxPrefab.transform.localScale = new Vector3(boardSize, boxSize, boxSize);
		boxPrefab.transform.localScale = new Vector3(boxSize, boxSize, boxSize);

		Vector3 pos = new Vector3(startBoxPosX, boxSize / 2, -1);
		Instantiate(startBoxPrefab, pos, Quaternion.identity, gameObject.transform);

        pos = new Vector3(startBoxPosX, boxSize / 2, boardSize);
		Instantiate(startBoxPrefab, pos, Quaternion.identity, gameObject.transform);

		// create game board
		for (int i = 0; i < boardSize; i++)
        {
            for(int j = 0; j < boardSize; j++)
            {
                // find the id of current box to see if it's solid
                int id = (( i ) * boardSize) + j + 1;
                bool isSolid = solidBoxIds.Contains(id);

				pos = new Vector3(j, boxSize / 2, i);

                GameObject box = Instantiate(boxPrefab, pos, Quaternion.identity, gameObject.transform);
                box.GetComponent<BoxController>().InitBox(isSolid, id);
                boxList.Add(box);
            }
        }

        return boxList;
    }
}
