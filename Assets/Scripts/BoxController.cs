using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.GameData;

public class BoxController : MonoBehaviour
{
	bool isEnabled = true;
	bool mouseDrag = false;
	Color highlightColor;
	Color selectColor;
	Color coverColor;
	Color hiddenColor;
    Renderer boxRenderer;

    // Start is called before the first frame update
    void Start()
    {
		boxRenderer = GetComponent<Renderer>();
		coverColor = boxRenderer.material.color;

		if(ColorDict?.Keys != null)
		{
			highlightColor = ColorDict[Colors.LightGray];
			selectColor = ColorDict[Colors.DarkGray];
			hiddenColor = BoxColors.ElementAt(Random.Range(0, BoxColors.Count));
		}

	}

    // Update is called once per frame
    void Update()
    {
        
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
		
	}

	private void OnMouseUp()
	{
		if (isEnabled)
		{
			isEnabled = false;
			boxRenderer.material.color = hiddenColor;
		}
	}
}
