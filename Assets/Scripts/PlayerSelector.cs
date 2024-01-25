using GameNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector : MonoBehaviour
{
	public List<GameObject> playerChars = new List<GameObject>();
	private static int selectedCharId = 0;

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
		int newId = (selectedCharId + 1) % 4;
		SetPlayer(newId);
    }

	public void SelectPrev()
	{
		int newId = selectedCharId < 1 ? 3 : selectedCharId - 1;
		SetPlayer(newId);
	}

	void SetPlayer(int charId)
    {
		playerChars[selectedCharId].SetActive(false);
		playerChars[charId].SetActive(true);

		selectedCharId = charId;
	}

	public static int getPlayerModelId()
	{
		return selectedCharId;
	}

	public GameObject InstantiateCharacter(int charId, Vector3 pos, Quaternion rotation)
	{
		GameObject characterPref = playerChars[charId];
		return Instantiate(characterPref, pos, rotation);
	}
}
