using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitGUI : MonoBehaviour {

	public StatsGUI stats;
	public Button sacrifice;
	private Unit unit;
	[SerializeField]private int player;

	// Update is called once per frame
	void Update () {
		TurnInfo();
		unit = Logic.Inst.SelectedUnit;
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
