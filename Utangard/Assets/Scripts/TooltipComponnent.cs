using UnityEngine;
using System.Collections;

public class TooltipComponnent : MonoBehaviour {

	public TooltipType type;
	float timer = 2.0f;
	float counter = 0;
	bool hovered;

	public void OnMouseOver(){
		if(counter >= timer){
			GUIManager.inst.ExpandTooltip(type);
			counter = 0;
		}
		else{
			counter += Time.deltaTime;
		}
	}

	public void OnMouseEnter(){
        if (type == TooltipType.unit)
            GUIManager.inst.OpenToolip(type, GetComponent<Unit>());
        else
            GUIManager.inst.OpenToolip(type, GetComponent<Tile>());
	}

	public void OnMouseExit(){
		GUIManager.inst.CloseTooltip();
		counter = 0;
	}

}
