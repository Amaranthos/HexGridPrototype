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
	public string actives;
	public bool isTaken;

	public HeroStruct(GameObject model, string name, string desc, string passive, string actives) {
		this.model = model;
		this.name = name;
		this.desc = desc;
		this.passive = passive;
		this.actives = actives;
		isTaken = false;
	}
}

public class NewHeroSelect : MonoBehaviour {

	public HeroStruct[] heroes;
	public GameObject[] indexArrows;
	public GameObject[] heroCircles;
	public int numHeroes;

	public GameObject armySelect;
	public Text[] textBoxes = new Text[4]; // 1 = Hero Name 2 = Hero Desc 3 = Passive Skill 4 = Abilities
	public Text selectionText;

	public HeroType selectedHero = HeroType.Skadi;
	Hero p1Hero;
	Hero p2Hero;

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
				heroCircles[(int)selectedHero].transform.GetChild(0).GetComponent<RawImage>().color = Color.grey;
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

	public void NextHero(){
		if(selectedHero != HeroType.Sam){
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
			selectedHero = HeroType.Sam;
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
		ToggleIndexArrows((int)selectedHero);
	}

	void SetInfo(int i){
		textBoxes[0].text = heroes[i].name; // Set Name
		textBoxes[1].text = heroes[i].desc; // Set Description Text
		textBoxes[2].text = heroes[i].passive; // Set Passive Text
		textBoxes[3].text = heroes[i].actives; // Set Abilities Text
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

	void ToggleIndexArrows(int i)
	{
		for(int j = 0; j < numHeroes; j++){
			if(j != i){
				indexArrows[j].SetActive(false);
			}
			else
				indexArrows[j].SetActive(true);
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

		// Grab Character Passives
		if(System.IO.File.Exists("Assets/GameData/PassiveAbilities.txt")){
			strings = fileCSV.GetCSV("Assets/GameData/PassiveAbilities.txt").Split(";"[0]); // Divide string into array elements, seperated by ;
			for(int i = 0; i < numHeroes; i++){
				heroes[i].passive = strings[i].Trim(); // Remove any newlines in the string
			}
		}
		else{
			Debug.LogError("Abilities.txt not found");
		}
		// Grab Character Abilities
		if(System.IO.File.Exists("Assets/GameData/Abilities.txt")){
			strings = fileCSV.GetCSV("Assets/GameData/Abilities.txt").Split(";"[0]); // Divide string into array elements, seperated by ;
			for(int i = 0; i < numHeroes; i++){
				heroes[i].actives = strings[i].Trim(); // Remove any newlines in the string
			}
		}
		else{
			Debug.LogError("Abilities.txt not found");
		}
	}
}