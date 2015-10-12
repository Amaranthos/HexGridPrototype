using UnityEngine;
using System.Collections;

public class UnitGUI : MonoBehaviour {

	public StatsGUI stats;
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
				stats.turninfo.text = (Logic.Inst.CurrentPlayerNum + 1) + "'s turn" + "\n" + Logic.Inst.gamePhase.ToString();
				stats.hp.text = unit.CurrentHitpoints.ToString();
				stats.moves.text = unit.CurrentMovePoints.ToString();
				stats.atk.text = unit.attack.ToString();
				stats.def.text = unit.defense.ToString();
				stats.dodge.text = unit.dodgeChance.ToString();
				stats.hit.text = unit.hitChance.ToString();
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
