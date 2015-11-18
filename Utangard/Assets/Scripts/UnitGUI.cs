using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitGUI : MonoBehaviour {

	public StatsGUI stats;
	public Button sacrifice;
	public GameObject[] abilities;
	private Unit unit;
	[SerializeField]private int player;

	void Start(){
		switch(Logic.Inst.Players[player].hero.type){
		case HeroType.Eir:
			abilities[0].GetComponent<Image>().sprite = GUIManager.inst.abilityIcons[0].ability1;
			abilities[1].GetComponent<Image>().sprite = GUIManager.inst.abilityIcons[0].ability2;
			break;
		case HeroType.Heimdal:
			abilities[0].GetComponent<Image>().sprite = GUIManager.inst.abilityIcons[1].ability1;
			abilities[1].GetComponent<Image>().sprite = GUIManager.inst.abilityIcons[1].ability2;
			break;
		case HeroType.Skadi:
			abilities[0].GetComponent<Image>().sprite = GUIManager.inst.abilityIcons[2].ability1;
			abilities[1].GetComponent<Image>().sprite = GUIManager.inst.abilityIcons[2].ability2;
			break;
		case HeroType.Thor:
			abilities[0].GetComponent<Image>().sprite = GUIManager.inst.abilityIcons[3].ability1;
			abilities[1].GetComponent<Image>().sprite = GUIManager.inst.abilityIcons[3].ability2;
			break;
		}
	}

	// Update is called once per frame
	void Update () {
		TurnInfo();
		unit = Logic.Inst.SelectedUnit;
		UpdateIcons();
		if(unit != null){
			UpdateStats();
		}
		else{
			WipeText();
		}
	}

	void UpdateStats(){
		if(unit != null){
			if(player == Logic.Inst.CurrentPlayerNum){
				TurnInfo();
				//stats.hp.text = unit.CurrentHitpoints.ToString() + "/"+ unit.maxHitpoints.ToString();
				HealthStat();
				stats.unitName.text = unit.type.ToString();
				stats.moves.text = unit.CurrentMovePoints.ToString();
				stats.atk.text = unit.TotalAttack.ToString();
				stats.def.text = unit.TotalDefense.ToString();
				stats.dodge.text = unit.TotalDodgeChance.ToString();
				stats.hit.text = unit.TotalHitChance.ToString();
				if(Logic.Inst.gamePhase == GamePhase.CombatPhase){
					if(unit.CanMove){
						Altar altar = Logic.Inst.GetAltar(unit.Index);
						if(altar){
							sacrifice.interactable = true;
						}
						else{
							sacrifice.interactable = false;
						}
					}
					else {
						sacrifice.interactable = false;
					}
				}
			}
		}

	}

	void UpdateIcons(){
		if(player == Logic.Inst.CurrentPlayerNum){
			if(unit != null){
				switch(unit.Owner.hero.type){
				case HeroType.Eir:
					if(unit.type != UnitType.Hero){
						switch(unit.type){
						case UnitType.Axemen:
							GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[0].axe;
							break;
						case UnitType.Spearman:
							GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[0].spear;
							break;
						case UnitType.Swordsmen:
							GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[0].sword;
							break;
						}
					}
					else{
						GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.heroPortraits[0];
					}				
					break;
				case HeroType.Heimdal:
					if(unit.type != UnitType.Hero){
					switch(unit.type){
						case UnitType.Axemen:
							GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[1].axe;
							break;
						case UnitType.Spearman:
							GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[1].spear;
							break;
						case UnitType.Swordsmen:
							GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[1].sword;
							break;
						}
					}
					else{
						GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.heroPortraits[1];
					}
					break;
				case HeroType.Skadi:
					if(unit.type != UnitType.Hero){
						switch(unit.type){
						case UnitType.Axemen:
							GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[2].axe;
							break;
						case UnitType.Spearman:
							GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[2].spear;
							break;
						case UnitType.Swordsmen:
							GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[2].sword;
							break;
						}
					}
					else{
						GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.heroPortraits[2];
					}
					break;
				case HeroType.Thor:
					if(unit.type != UnitType.Hero){
						switch(unit.type){
						case UnitType.Axemen:
							GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[3].axe;
							break;
						case UnitType.Spearman:
							GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[3].spear;
							break;
						case UnitType.Swordsmen:
							GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[3].sword;
							break;
						}
					}
					else{
						GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.heroPortraits[3];
					}
					break;
				}
			}
			else{
				switch(Logic.Inst.Players[player].hero.type){
				case HeroType.Eir:
					GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[0].none;
					break;
				case HeroType.Heimdal:
					GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[1].none;
					break;
				case HeroType.Skadi:
					GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[2].none;
					break;
				case HeroType.Thor:
					GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[3].none;
					break;
				}
			}
		}
		else{
			// print ("Getting Hit");
			switch(Logic.Inst.Players[player].hero.type){
			case HeroType.Eir:
				GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[0].none;
				break;
			case HeroType.Heimdal:
				GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[1].none;
				break;
			case HeroType.Skadi:
				GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[2].none;
				break;
			case HeroType.Thor:
				GUIManager.inst.playerAvatars[player].sprite = GUIManager.inst.unitPortraits[3].none;
				break;
			}
		}
	}


	void WipeText(){
		stats.hp.text = "";
		stats.unitName.text = "";
		stats.moves.text = "";
		stats.atk.text = "";
		stats.def.text = "";
		stats.dodge.text = "";
		stats.hit.text = "";
	}

	void HealthStat(){
		if(unit.CurrentHitpoints < (unit.maxHitpoints / 4)){
			stats.hp.text = "<b><color=red>" + unit.CurrentHitpoints.ToString() + "</color></b>/"+ unit.maxHitpoints.ToString();
		}
		else{
			stats.hp.text = unit.CurrentHitpoints.ToString() + "|"+ unit.maxHitpoints.ToString();
		}
	}

	void TurnInfo(){
		if(Logic.Inst.gamePhase == GamePhase.PlacingPhase){
			stats.turninfo.text = "Deployment Phase";
		}
		else if(Logic.Inst.gamePhase == GamePhase.CombatPhase){
			stats.turninfo.text = "Player " + (Logic.Inst.CurrentPlayerNum + 1) + "'s turn";
			stats.turninfo.text += "\n" + "Faith: " + Logic.Inst.CurrentPlayer.Faith.ToString();
			stats.turninfo.text += " (<color=lime>+" + Logic.Inst.FaithPerTurn + "</color>)";
		}

	}

	
}
