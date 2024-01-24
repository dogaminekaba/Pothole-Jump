using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.GameData;

public class BoxController : MonoBehaviour
{
	bool isBoxSolid = false;
	bool isEnabled = true;
	bool mouseDrag = false;
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

		boxRenderer = gameObject.GetComponent<Renderer>();

		if (ColorDict?.Keys != null)
		{
			coverColor = boxRenderer.material.color;
			highlightColor = ColorDict[Colors.LightGray];
			selectColor = ColorDict[Colors.DarkGray];

			if (isSolid)
				hiddenColor = ColorDict[Colors.White];
			else
				hiddenColor = BoxColors.ElementAt(Random.Range(0, BoxColors.Count));
		}

		
	}

	private void OnMouseEnter()
	{
		if(isEnabled)
		{
			boxRenderer.material.color = highlightColor;
		}
	}

	private void OnMouseExit()
	{
		if (isEnabled)
		{
			boxRenderer.material.color = coverColor;
		}
	}

	private void OnMouseDown()
	{
		if (isEnabled)
		{
			boxRenderer.material.color = selectColor;
		}
	}

	private void OnMouseDrag()
	{
		mouseDrag = false;
	}

	private void OnMouseUp()
	{
		if (isEnabled && !mouseDrag)
		{
			isEnabled = false;
			boxRenderer.material.color = hiddenColor;

			Debug.Log("id: " + boxId);
		}
		else
		{
			mouseDrag = false;
		}	
	}
}
