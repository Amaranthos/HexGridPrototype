using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BalanceBar : MonoBehaviour {

	Slider bar;
	public Image background;
	public Image fill;
	public Image[] heroImages = new Image[2];
	public Sprite[] heroPortraits = new Sprite[4];


	// Use this for initialization
	void Start () {
		bar = GetComponentInChildren<Slider>();
		bar.value = Logic.Inst.Players[0].capturedAltars.Count;
		background.color = Logic.Inst.Players[1].playerColour;
		fill.color = Logic.Inst.Players[0].playerColour;
		AssignPortraits();
	}
	
	// Update is called once per frame
	void Update () {
		bar.value = Logic.Inst.Players[0].capturedAltars.Count;
	}

	void AssignPortraits(){
		switch(Logic.Inst.Players[0].hero.type){
		case HeroType.Eir:
			heroImages[0].sprite = heroPortraits[0];
			break;
		case HeroType.Heimdal:
			heroImages[0].sprite = heroPortraits[1];
			break;
		case HeroType.Skadi:
			heroImages[0].sprite = heroPortraits[2];
			break;
		case HeroType.Thor:
			heroImages[0].sprite = heroPortraits[3];
			break;
		}
	
		switch(Logic.Inst.Players[1].hero.type){
		case HeroType.Eir:
			heroImages[1].sprite = heroPortraits[0];
			break;
		case HeroType.Heimdal:
			heroImages[1].sprite = heroPortraits[1];
			break;
		case HeroType.Skadi:
			heroImages[1].sprite = heroPortraits[2];
			break;
		case HeroType.Thor:
			heroImages[1].sprite = heroPortraits[3];
			break;
		}
	}
}
