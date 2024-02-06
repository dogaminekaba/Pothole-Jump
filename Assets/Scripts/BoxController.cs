using System.Linq;
using UnityEngine;
using static Assets.Scripts.GameData;

public class BoxController : MonoBehaviour
{
	public int hiddenColorIndex;

	bool isBoxSolid = false;
	bool isEnabled = true;
	Color highlightColor;
	Color selectColor;
	Color coverColor;
	Color hiddenColor;
    Renderer boxRenderer;
	int boxId = -1;

    // Start is called before the first frame update
    void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void InitBox(bool isSolid, int id)
	{
		isBoxSolid = isSolid;
		boxId = id;

		if (ColorDict?.Keys != null)
		{
			boxRenderer = gameObject.GetComponent<Renderer>();
			coverColor = boxRenderer.material.color;
			highlightColor = ColorDict[Colors.DarkGray];
			selectColor = ColorDict[Colors.DarkestGray];

			if (isSolid)
			{
				hiddenColorIndex = 0;
			}
			else
			{
				hiddenColorIndex = Mathf.Max(1, boxId % BoxColors.Count);
				hiddenColor = BoxColors[hiddenColorIndex];
			}

			hiddenColor = BoxColors[hiddenColorIndex];

			boxRenderer.material.color = coverColor;
		}
	}

	public void RevealColor()
	{
		boxRenderer.material.color = hiddenColor;
		isEnabled = false;
	}

	private void OnMouseEnter()
	{
		if(isEnabled && GameManager.canPlay)
		{
			boxRenderer.material.color = highlightColor;
		}
	}

	private void OnMouseExit()
	{
		if (isEnabled && GameManager.canPlay)
		{
			boxRenderer.material.color = coverColor;
		}
	}

	private void OnMouseDown()
	{
		if (isEnabled && GameManager.canPlay)
		{
			boxRenderer.material.color = selectColor;
		}
	}

	private void OnMouseUp()
	{
		if (isEnabled && GameManager.canPlay)
		{
			isEnabled = false;
			boxRenderer.material.color = hiddenColor;

			Debug.Log("id: " + boxId);

			GameManager.UpdateCurrentBox(boxId, hiddenColorIndex);
		}
	}
}
