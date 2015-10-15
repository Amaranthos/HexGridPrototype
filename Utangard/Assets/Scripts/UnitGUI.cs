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
				stats.turninfo.text = "Player " + (Logic.Inst.CurrentPlayerNum + 1) + "'s turn" + "\n" + Logic.Inst.gamePhase.ToString();
				stats.hp.text = unit.CurrentHitpoints.ToString() + "/"+ unit.maxHitpoints.ToString();
				stats.moves.text = unit.CurrentMovePoints.ToString();
				stats.atk.text = unit.attack.ToString();
				stats.def.text = unit.defense.ToString();
				stats.dodge.text = unit.dodgeChance.ToString();
				stats.hit.text = unit.hitChance.ToString();
				if(Logic.Inst.gamePhase == GamePhase.CombatPhase){
					if(unit.CanMove){
						Altar altar = Logic.Inst.GetAltar(unit.Index);
						if(altar){
							sacrifice.interactable = true;
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
		stats.moves.text = "";
		stats.atk.text = "";
		stats.def.text = "";
		stats.dodge.text = "";
		stats.hit.text = "";
	}
	
}
