using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattlePredictionUI : MonoBehaviour {

	[System.Serializable]
	public struct PredictionText{
		public Text chanceText;
		public Text breakdownText;
	};

	public GameObject[] panels = new GameObject[3];
	public StatsGUI[] predictionStats = new StatsGUI[2];
	public PredictionText[] predictText = new PredictionText[2];
	public bool isOpen;
	private Unit hoverUnit;
	private Image image;
	private float magicX, magicY;

	void Start(){
		image = GetComponent<Image>();
		magicX = Logic.Inst.CombatManager.defNumA;
		magicY = Logic.Inst.CombatManager.defNumB;
	}

	// Update is called once per frame
	void Update () {
		if(Logic.Inst.gamePhase == GamePhase.CombatPhase){
			if(isOpen){
				UpdateStats();
				UpdateBreakdown();
			}
			else{
				ClearStats();
			}
		}
	}

	public void OpenPredictions(Unit hoveredUnit){
		isOpen = true;
		hoverUnit = hoveredUnit;
		image.enabled = true;
		panels[0].SetActive(true);
		panels[1].SetActive(true);
		panels[2].SetActive(true);
	}

	public void ClosePredictions(){
		isOpen = false;
		hoverUnit = null;
		image.enabled = false;
		panels[0].SetActive(false);
		panels[1].SetActive(false);
		panels[2].SetActive(false);
	}

	void UpdateBreakdown(){
		// 0 == attacker, 1 == defender
		predictText[0].chanceText.text = ChanceToHit(0).ToString() + "% to hit for " + DamageDealt(0).ToString() + " damage";
		predictText[1].chanceText.text = ChanceToHit(1).ToString() + "% to hit for " + DamageDealt(1).ToString() + " damage";

		//wipe text
		predictText[0].breakdownText.text = "";
		predictText[1].breakdownText.text = "";

		//List Buffs
		for(int i = 0; i < Logic.Inst.SelectedUnit.currentBuffs.Count; i++){
			predictText[0].breakdownText.text += Logic.Inst.SelectedUnit.currentBuffs[i].buffType + " ";
			predictText[0].breakdownText.text += Logic.Inst.SelectedUnit.currentBuffs[i].strength + "\n";
		}
		for(int i = 0; i < hoverUnit.currentBuffs.Count; i++){
			predictText[0].breakdownText.text += hoverUnit.currentBuffs[i].buffType + " ";
			predictText[0].breakdownText.text += hoverUnit.currentBuffs[i].strength + "\n";
		}
	}

	int ChanceToHit(int attacker){ // 0 == attacker, 1 == defender
		if(attacker == 0){
			return Logic.Inst.SelectedUnit.hitChance - hoverUnit.dodgeChance;
		}
		else{
			return hoverUnit.hitChance - Logic.Inst.SelectedUnit.dodgeChance;
		}
	}

	int DamageDealt(int attacker){ // 0 == attacker, 1 == defender
		if(attacker == 0){
			return (int)(Logic.Inst.SelectedUnit.attack - (1 -((magicX * hoverUnit.defense)/(magicY + hoverUnit.defense))));
		}
		else{
			return (int)(hoverUnit.attack - (1 -((magicX * Logic.Inst.SelectedUnit.defense)/(magicY + Logic.Inst.SelectedUnit.defense))));
		}
	}

	void UpdateStats(){
		// update Attacker stats
		predictionStats[0].unitName.text = Logic.Inst.SelectedUnit.name;
		predictionStats[0].hp.text = Logic.Inst.SelectedUnit.CurrentHitpoints.ToString();
		predictionStats[0].atk.text = Logic.Inst.SelectedUnit.attack.ToString();
		predictionStats[0].def.text = Logic.Inst.SelectedUnit.defense.ToString();
		predictionStats[0].hit.text = Logic.Inst.SelectedUnit.hitChance.ToString();
		predictionStats[0].dodge.text = Logic.Inst.SelectedUnit.dodgeChance.ToString();
		
		// update Defender Stats
		predictionStats[1].unitName.text = hoverUnit.name;
		predictionStats[1].hp.text = hoverUnit.CurrentHitpoints.ToString();
		predictionStats[1].atk.text = hoverUnit.attack.ToString();
		predictionStats[1].def.text = hoverUnit.defense.ToString();
		predictionStats[1].hit.text = hoverUnit.hitChance.ToString();
		predictionStats[1].dodge.text = hoverUnit.dodgeChance.ToString();
	}

	void ClearStats(){
		predictionStats[0].unitName.text = "";
		predictionStats[0].hp.text = "";
		predictionStats[0].atk.text = "";
		predictionStats[0].def.text = "";
		predictionStats[0].hit.text = "";
		predictionStats[0].dodge.text = "";

		predictionStats[1].unitName.text = "";
		predictionStats[1].hp.text = "";
		predictionStats[1].atk.text = "";
		predictionStats[1].def.text = "";
		predictionStats[1].hit.text = "";
		predictionStats[1].dodge.text = "";
	}
}
