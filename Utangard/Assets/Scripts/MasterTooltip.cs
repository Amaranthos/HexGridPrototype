using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MasterTooltip : MonoBehaviour {

	public TooltipPanel[] tooltipPanels;
	public Vector2 tipOffset = new Vector2();
	//RectTransform rect;
	Vector3 cursorPos = new Vector3(0,0,100);

	void Start () 
	{
	//	rect =  GetComponent<RectTransform>();
		cursorPos.x = Input.mousePosition.x + tipOffset.x;
		cursorPos.y = Input.mousePosition.y + tipOffset.y;
	}

	// Update is called once per frame
	void LateUpdate () 
	{
		cursorPos.x = Input.mousePosition.x + tipOffset.x;
		cursorPos.y = Input.mousePosition.y + tipOffset.y;
		transform.position = Camera.main.ScreenToWorldPoint(cursorPos);
	}

	public void ActivateTooltip(TooltipType type, Unit hoverUnit)
	{
		switch(type)
		{
		case TooltipType.unit:
			print ("Unit Tooltip Open");
			tooltipPanels[0].hoverUnit = hoverUnit;
			tooltipPanels[0].TurnOn();
			break;
		}
	}

	public void ExpandTooltip(TooltipType type){
		switch(type){
		case TooltipType.unit:
			tooltipPanels[0].ExpandTip();
			break;
		}

	}

	public void DeactivateTooltips()
	{
		tooltipPanels[0].TurnOff();
		tooltipPanels[0].CloseTip();
		tooltipPanels[0].hoverUnit = null;
	}

}
