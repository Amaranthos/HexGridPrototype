using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct HeroStruct {
	public GameObject model;
	public string name;
	public string desc;
	public string passive;
	public string active1;
	public string active2;
	public bool isTaken;

	public HeroStruct(GameObject model, string name, string desc, string passive, string active1, string active2) {
		this.model = model;
		this.name = name;
		this.desc = desc;
		this.passive = passive;
		this.active1 = active1;
		this.active2 = active2;
		isTaken = false;
	}
}

public class NewHeroSelect : MonoBehaviour {

	public HeroStruct[] heroes;
	//public GameObject[] indexArrows;
	public GameObject[] heroCircles;
	public int numHeroes;

	public GameObject armySelect;
	public Text[] textBoxes = new Text[5]; // 1 = Hero Name 2 = Hero Desc 3 = Passive Skill 4 = Ability1 5 = Ability2
	public Text selectionText;

	public HeroType selectedHero = HeroType.Skadi;
	Hero p1Hero;
	Hero p2Hero;
	public Color notSelectedColor = new Color(129,129,129);
	public Color takenColor;

	private CSVParser fileCSV = new CSVParser();

	bool p1HasChosen = false;


	void Start(){
		numHeroes = Logic.Inst.HeroList.heroes.Count;
		//heroes = new HeroStruct[numHeroes];
		LoadDataFromFile();
		UpdateHeroInfo();
	}

	public void ConfirmSelection(){
		if(!isTaken((int)selectedHero)){
			if(!p1HasChosen){
				p1HasChosen = true;
				p1Hero = Logic.Inst.HeroList.GetHero(selectedHero).GetComponent<Hero>();
				heroCircles[(int)selectedHero].GetComponent<Image>().color = takenColor;
				selectionText.text = "Player 2 Choosing";
			}
			else{
				p2Hero = Logic.Inst.HeroList.GetHero(selectedHero).GetComponent<Hero>();
				Logic.Inst.Players[0].hero = p1Hero;
				Logic.Inst.Players[1].hero = p2Hero;
				armySelect.SetActive(true);
				gameObject.SetActive(false);
			}
		}
	}

	public void SetHeroIndex(int heroNum){
		selectedHero = (HeroType)heroNum;
		UpdateHeroInfo();
	}

	public void NextHero(){
		if(selectedHero != HeroType.Thor){
			selectedHero += 1;
			UpdateHeroInfo();
		}
		else{
			selectedHero = HeroType.Eir;
			UpdateHeroInfo();
		}
	}

	public void PreviousHero(){
		if(selectedHero != HeroType.Eir){
			selectedHero -= 1;
			UpdateHeroInfo();
		}
		else{
			selectedHero = HeroType.Thor;
			UpdateHeroInfo();
		}
	}

	bool isTaken(int hero){
		bool ret = heroes[hero].isTaken;

		if(!heroes[hero].isTaken)
			heroes[hero].isTaken = true;

		return ret;
	}

	void UpdateHeroInfo(){
		ToggleModels((int)selectedHero);
		SetInfo((int)selectedHero);
		ToggleCircleColors((int)selectedHero);
	}

	void SetInfo(int i){
		textBoxes[0].text = heroes[i].name; // Set Name
		textBoxes[1].text = heroes[i].desc; // Set Description Text
		textBoxes[2].text = heroes[i].passive; // Set Passive Text
		textBoxes[3].text = heroes[i].active1; // Set Ability 1 Text
		textBoxes[4].text = heroes[i].active2; // Set Ability 2 Text
	}

	void ToggleModels(int i){
		for(int j = 0; j < numHeroes; j++){
			if(j != i){
				heroes[j].model.SetActive(false);
			}
			else
				heroes[j].model.SetActive(true);
		}
	}

	void ToggleCircleColors(int i)
	{
		for(int j = 0; j < numHeroes; j++){
			if(j != i){
				if(!heroes[j].isTaken){
				heroCircles[(int)j].GetComponent<Image>().color = notSelectedColor;
				}
				//indexArrows[j].SetActive(false);
			}
			else{
				if(!heroes[j].isTaken){
					heroCircles[(int)j].GetComponent<Image>().color = Color.white;
				}
				//indexArrows[j].SetActive(true);
			}
		}
	}

	void LoadDataFromFile(){
		string[] strings = new string[numHeroes];

		// Grab Character Descriptions
		if(System.IO.File.Exists("Assets/GameData/CharacterDescriptions.txt")){
			strings = fileCSV.GetCSV("Assets/GameData/CharacterDescriptions.txt").Split(";"[0]); // Divide string into array elements, seperated by ;
			for(int i = 0; i < numHeroes; i++){
				heroes[i].desc = strings[i].Trim(); // Remove any newlines in the string
			}
		}
		else{
			Debug.LogError("Abilities.txt not found");
		}

		string[] strings2 = new string[5];
		// Grab Ability Descriptions
		if(System.IO.File.Exists("Assets/GameData/AbilityTooltips.txt")){
			strings2 = fileCSV.GetCSV("Assets/GameData/AbilityTooltips.txt").Split(";"[0]); // Divide string into array elements, seperated by ;
			for(int i = 0; i < (strings2.Length - 1); i++){
				string[] temp = new string[3];
				temp = strings2[i].Split("$"[0]);
				heroes[i].passive = temp[0].Trim();
				heroes[i].active1 = temp[1].Trim();
				heroes[i].active2 = temp[2].Trim();
			}
		}
		else{
			Debug.LogError("AbilityTooltips.txt not found");
		}


//		// Grab Character Passives
//		if(System.IO.File.Exists("Assets/GameData/PassiveAbilities.txt")){
//			strings = fileCSV.GetCSV("Assets/GameData/PassiveAbilities.txt").Split(";"[0]); // Divide string into array elements, seperated by ;
//			for(int i = 0; i < numHeroes; i++){
//				heroes[i].passive = strings[i].Trim(); // Remove any newlines in the string
//			}
//		}
//		else{
//			Debug.LogError("Abilities.txt not found");
//		}
//		// Grab Character Abilities
//		if(System.IO.File.Exists("Assets/GameData/Abilities.txt")){
//			strings = fileCSV.GetCSV("Assets/GameData/Abilities.txt").Split(";"[0]); // Divide string into array elements, seperated by ;
//			for(int i = 0; i < numHeroes; i++){
//				heroes[i].actives = strings[i].Trim(); // Remove any newlines in the string
//			}
//		}
//		else{
//			Debug.LogError("Abilities.txt not found");
//		}
	}
}