using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector : MonoBehaviour
{
	public List<GameObject> playerChars = new List<GameObject>();
	int selectedPlayerId = 0;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectNext()
    {
		int newId = (selectedPlayerId + 1) % 4;
		SetPlayer(newId);
    }

	public void SelectPrev()
	{
		int newId = selectedPlayerId < 1 ? 3 : selectedPlayerId - 1;
		SetPlayer(newId);
	}

	void SetPlayer(int playerId)
    {
		playerChars[selectedPlayerId].SetActive(false);
		playerChars[playerId].SetActive(true);

		selectedPlayerId = playerId;
	}
}
