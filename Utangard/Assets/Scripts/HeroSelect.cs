using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeroSelect : MonoBehaviour {
	public HeroSelectGuiHolder[] heroes;
	public GameObject armySelect;

	public List<HeroSelectStruct> available;

	public Toggle p1;
	public Toggle p2;
	public Button start;

	public void Start() {
		for (int i = 0; i < Logic.Inst.HeroList.heroes.Count; i++)
			available.Add(new HeroSelectStruct(Logic.Inst.HeroList.heroes[i]));

		for(int i = 0; i < heroes.Length; i++){
			heroes[i].Selected = available.Find(item => item.isSelected == false);
			heroes[i].Selected.isSelected = true;
			heroes[i].HeroSelect = this;
			heroes[i].Player = Logic.Inst.Players[i];
			heroes[i].Init();
		}
	}

	public void Update() {
		if (p1.isOn && p2.isOn)
			start.interactable = true;
	}

	public void Finalise() {
		for(int i = 0; i < heroes.Length; i++){
			Logic.Inst.Players[i].hero = heroes[i].Selected.hero;
		}

		armySelect.SetActive(true);
		gameObject.SetActive(false);
	}
}

[System.Serializable]
public class HeroSelectGuiHolder {
	public Text title;
	public Text hero;
	public Text desc;
	public Button neg;
	public Button pos;

	private int index;

	public HeroSelectStruct Selected {get; set;}
	public Player Player {get; set;}
	public HeroSelect HeroSelect{get; set;}

	public void Init() {
		HeroSelectGuiHolder holder = this;

		neg.onClick.AddListener(delegate {
			holder.Decrement();
		});

		pos.onClick.AddListener(delegate {
			holder.Increment();
		});

		title.text = Player.playerName;

		index = HeroSelect.available.IndexOf(Selected);

		UpdateText();
	}

	public void Decrement() {
		DecrementIndex();

		Selected.isSelected = false;
		Selected = HeroSelect.available[index];
		Selected.isSelected = true;

		UpdateText();
	}

	public void DecrementIndex() {
		if (index > 0)			
			index--;				
		else
			index = HeroSelect.available.Count - 1;

		if (HeroSelect.available[index].isSelected)
			DecrementIndex();
	}

	public void Increment() {
		IncrementIndex();

		Selected.isSelected = false;
		Selected = HeroSelect.available[index];
		Selected.isSelected = true;

		UpdateText();
	}

	public void IncrementIndex() {
		if (index + 1 < HeroSelect.available.Count)
			index++;
		else
			index = 0;

		if (HeroSelect.available[index].isSelected)
			IncrementIndex();
	}

	private void UpdateText() {
		if (Selected.hero) {
			hero.text = Selected.hero.type.ToString();
			desc.text = "Filler text, needs to load relevant desc for hero";
		}
	}
}

[System.Serializable]
public class HeroSelectStruct {
	public Hero hero;
	public bool isSelected = false;

	public HeroSelectStruct(Hero hero) {
		this.hero = hero;
	}
}