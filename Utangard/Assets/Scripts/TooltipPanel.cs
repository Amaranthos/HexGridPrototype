using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TooltipPanel : MonoBehaviour {

	public TooltipType type;
	public GameObject[] thingsToTurnOff;
	// Unit: 0 = Name, 1 = HP, 2 = moves, 3 = atk, 4 = def, 5 = hit, 6 = dodge
	public Text[] textFields;
	public Unit hoverUnit;
	public bool Open;
	Animator anim;

	void Start(){
		anim = GetComponent<Animator>();
	}

	void Update(){
		if(Open){
			Behave();
		}
	}

	void Behave(){
		switch(type)
		{
		case TooltipType.unit:
			if(hoverUnit != null){
				textFields[0].text = hoverUnit.name;
				textFields[1].text = hoverUnit.CurrentHitpoints.ToString();
				textFields[2].text = hoverUnit.CurrentMovePoints.ToString();
				textFields[3].text = hoverUnit.TotalAttack.ToString();
				textFields[4].text = hoverUnit.TotalDefense.ToString();
				textFields[5].text = hoverUnit.TotalHitChance.ToString();
				textFields[6].text = hoverUnit.TotalDodgeChance.ToString();
			}
			break;
		}
	}

	public void TurnOn(){
		Open = true;
		thingsToTurnOff[0].GetComponent<Image>().enabled = true;
		thingsToTurnOff[1].SetActive(true);
		thingsToTurnOff[2].SetActive(true);
		thingsToTurnOff[3].SetActive(true);
	}
	public void TurnOff(){
		Open = false;
		thingsToTurnOff[0].GetComponent<Image>().enabled = false;
		thingsToTurnOff[1].SetActive(false);
		thingsToTurnOff[2].SetActive(false);
		thingsToTurnOff[3].SetActive(false);
	}

	public void ExpandTip(){
		Expand = true;
	}
	public void CloseTip(){
		Expand = false;
	}

	bool Expand{
		get{ return anim.GetBool("Expand"); }
		set{ anim.SetBool("Expand", value); }
	}

}
