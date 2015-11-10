using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityDescriptions : MonoBehaviour {

	public int playerNum;
	public GameObject toolTip;
	public string[] descriptions = new string[3];

	public Text toolTipText;
	CSVParser file = new CSVParser();

	// Use this for initialization
	void Start () {
		//toolTipText = toolTip.GetComponentInChildren<Text>();
		//LoadDescriptions();
		descriptions[0] = Logic.Inst.Players[playerNum].hero.GetPassiveText();
		descriptions[1] = Logic.Inst.Players[playerNum].hero.GetAbility1Text();
		descriptions[2] = Logic.Inst.Players[playerNum].hero.GetAbility2Text();
	}

	public void DisplayAbility1(){
		descriptions[1] = Logic.Inst.Players[playerNum].hero.GetAbility1Text();
		toolTip.SetActive(true);
		toolTipText.text = descriptions[1];
	}
	public void DisplayAbility2(){
		descriptions[2] = Logic.Inst.Players[playerNum].hero.GetAbility2Text();
		toolTip.SetActive(true);
		toolTipText.text = descriptions[2];
	}

	public void CloseTooltip(){
		toolTip.SetActive(false);
		toolTipText.text = "";
	}

	public void LoadDescriptions(){
		string[] strings = new string[5];
		bool succeeded = false;
		// Grab Ability Descriptions
		if(System.IO.File.Exists("Assets/GameData/AbilityTooltips.txt")){
			strings = file.GetCSV("Assets/GameData/AbilityTooltips.txt").Split(";"[0]); // Divide string into array elements, seperated by ;
			succeeded = true;
		}
		else{
			Debug.LogError("AbilityTooltips.txt not found");
			succeeded = false;
		}

		if(succeeded){
			switch(Logic.Inst.Players[playerNum].hero.type){
			case HeroType.Eir:
				descriptions = strings[0].Split("$"[0]);
				descriptions[1] = descriptions[0].Trim();
				descriptions[2] = descriptions[1].Trim();
				break;
			case HeroType.Heimdal:
				descriptions = strings[1].Split("$"[0]);
				descriptions[1] = descriptions[0].Trim();
				descriptions[2] = descriptions[1].Trim();
				break;
			case HeroType.Skadi:
				descriptions = strings[2].Split("$"[0]);
				descriptions[1] = descriptions[0].Trim();
				descriptions[2] = descriptions[1].Trim();
				break;
			case HeroType.Thor:
				descriptions = strings[3].Split("$"[0]);
				descriptions[1] = descriptions[0].Trim();
				descriptions[2] = descriptions[1].Trim();
				break;
			case HeroType.Sam:
				descriptions = strings[4].Split("$"[0]);
				descriptions[1] = descriptions[0].Trim();
				descriptions[2] = descriptions[1].Trim();
				break;
			}
		}

	}
}
