using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityDescriptions : MonoBehaviour {

	public int playerNum;
	public GameObject toolTip;
	public Button[] buttons;
	public string[] descriptions = new string[3];
	public Sprite[] icons = new Sprite[2];
	public Image icon;

	public Text toolTipText;
	CSVParser file = new CSVParser();

	// Use this for initialization
	void Start () {
		//toolTipText = toolTip.GetComponentInChildren<Text>();
		//LoadDescriptions();
		descriptions[0] = Logic.Inst.Players[playerNum].hero.GetPassiveText();
		descriptions[1] = Logic.Inst.Players[playerNum].hero.GetAbility1Text();
		descriptions[2] = Logic.Inst.Players[playerNum].hero.GetAbility2Text();
		//set Icons
		switch(Logic.Inst.Players[playerNum].hero.type){
		case HeroType.Eir:
			icons[0] = GUIManager.inst.abilityIcons[0].ability1;
			icons[1] = GUIManager.inst.abilityIcons[0].ability2;
			break;
		case HeroType.Heimdal:
			icons[0] = GUIManager.inst.abilityIcons[1].ability1;
			icons[1] = GUIManager.inst.abilityIcons[1].ability2;
			break;
		case HeroType.Skadi:
			icons[0] = GUIManager.inst.abilityIcons[2].ability1;
			icons[1] = GUIManager.inst.abilityIcons[2].ability2;
			break;
		case HeroType.Thor:
			icons[0] = GUIManager.inst.abilityIcons[3].ability1;
			icons[1] = GUIManager.inst.abilityIcons[3].ability2;
			break;
		}
	}

	public void Update(){
		if(Logic.Inst.Players[playerNum].Faith > Logic.Inst.Players[playerNum].hero.active1.cost){
			buttons[0].interactable = true;
		}
		else{
			buttons[0].interactable = false;
		}

		if(Logic.Inst.Players[playerNum].Faith > Logic.Inst.Players[playerNum].hero.active2.cost){
			buttons[1].interactable = true;
		}
		else{
			buttons[1].interactable = false;
		}
	}

	public void DisplayAbility1(){
		descriptions[1] = Logic.Inst.Players[playerNum].hero.GetAbility1Text();
		icon.sprite = icons[0];
		toolTip.SetActive(true);
		toolTipText.text = descriptions[1];
	}
	public void DisplayAbility2(){
		descriptions[2] = Logic.Inst.Players[playerNum].hero.GetAbility2Text();
		icon.sprite = icons[1];
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
