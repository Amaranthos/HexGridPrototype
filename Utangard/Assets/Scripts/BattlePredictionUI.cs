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
	public Image image;
	public bool isOpen;
	private Unit hoverUnit;
	private float magicX, magicY;

	void Start(){
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
		predictText[0].chanceText.text = "Hit Rate: " + ChanceToHit(0).ToString() + "%";
		predictText[1].chanceText.text = "Hit Rate: " + ChanceToHit(1).ToString() + "%";

		//wipe text
		predictText[0].breakdownText.text = "";
		predictText[1].breakdownText.text = "";

		//List Buffs
		for(int i = 0; i < Logic.Inst.SelectedUnit.currentBuffs.Count; i++){
			if(Logic.Inst.SelectedUnit.currentBuffs[i].debuff){
				predictText[0].breakdownText.text += "<color=red>";
				predictText[0].breakdownText.text += Logic.Inst.SelectedUnit.currentBuffs[i].buffType + " ";
				predictText[0].breakdownText.text += Logic.Inst.SelectedUnit.currentBuffs[i].strength + "\n";
				predictText[0].breakdownText.text += "</color>";
			}
			else{
				predictText[0].breakdownText.text += "<color=green>";
				predictText[0].breakdownText.text += Logic.Inst.SelectedUnit.currentBuffs[i].buffType + " ";
				predictText[0].breakdownText.text += Logic.Inst.SelectedUnit.currentBuffs[i].strength + "\n";
				predictText[0].breakdownText.text += "</color>";
			}
		}
		for(int i = 0; i < hoverUnit.currentBuffs.Count; i++){
			if(hoverUnit.currentBuffs[i].debuff){
				predictText[1].breakdownText.text += "<color=red>";
				predictText[1].breakdownText.text += hoverUnit.currentBuffs[i].buffType + " ";
				predictText[1].breakdownText.text += hoverUnit.currentBuffs[i].strength + "\n";
				predictText[1].breakdownText.text += "</color>";
			}
			else{
				predictText[1].breakdownText.text += "<color=green>";
				predictText[1].breakdownText.text += hoverUnit.currentBuffs[i].buffType + " ";
				predictText[1].breakdownText.text += hoverUnit.currentBuffs[i].strength + "\n";
				predictText[1].breakdownText.text += "</color>";
			}
		}
	}

	int ChanceToHit(int attacker){ // 0 == attacker, 1 == defender
		if(attacker == 0){
			return Logic.Inst.SelectedUnit.TotalHitChance - hoverUnit.TotalDodgeChance;
		}
		else{
			return hoverUnit.TotalHitChance - Logic.Inst.SelectedUnit.TotalDodgeChance;
		}
	}

	int DamageDealt(int attacker){ // 0 == attacker, 1 == defender
		if(attacker == 0){
			return (int)(Logic.Inst.SelectedUnit.TotalAttack - (1 -((magicX * hoverUnit.TotalDefense)/(magicY + hoverUnit.TotalDefense))));
		}
		else{
			return (int)(hoverUnit.TotalAttack - (1 -((magicX * Logic.Inst.SelectedUnit.TotalDefense)/(magicY + Logic.Inst.SelectedUnit.TotalDefense))));
		}
	}

	void UpdateStats(){
		// update Attacker stats
		predictionStats[0].unitName.text = Logic.Inst.SelectedUnit.name;
		predictionStats[0].hp.text = Logic.Inst.SelectedUnit.CurrentHitpoints.ToString() + " > " +
			(Logic.Inst.SelectedUnit.CurrentHitpoints - DamageDealt(1)) + " (<color=red>-" + DamageDealt(1) + "</color>)";

		if(Logic.Inst.SelectedUnit.TotalAttack > Logic.Inst.SelectedUnit.attack){
			predictionStats[0].atk.text = "<color=green>" + Logic.Inst.SelectedUnit.TotalAttack.ToString() + "</color>";
		}
		else if(Logic.Inst.SelectedUnit.TotalAttack < Logic.Inst.SelectedUnit.attack){
			predictionStats[0].atk.text = "<color=red>" + Logic.Inst.SelectedUnit.TotalAttack.ToString() + "</color>";
		}
		else{
			predictionStats[0].atk.text = Logic.Inst.SelectedUnit.TotalAttack.ToString();
		}
		
		if(Logic.Inst.SelectedUnit.TotalDefense > Logic.Inst.SelectedUnit.defense){
			predictionStats[0].def.text = "<color=green>" + Logic.Inst.SelectedUnit.TotalDefense.ToString() + "</color>";
		}
		else if(Logic.Inst.SelectedUnit.TotalDefense < Logic.Inst.SelectedUnit.defense){
			predictionStats[0].def.text = "<color=red>" + Logic.Inst.SelectedUnit.TotalDefense.ToString() + "</color>";
		}
		else{
			predictionStats[0].def.text = Logic.Inst.SelectedUnit.TotalDefense.ToString();
		}
		
		if(Logic.Inst.SelectedUnit.TotalHitChance > Logic.Inst.SelectedUnit.hitChance){
			predictionStats[0].hit.text = "<color=green>" + Logic.Inst.SelectedUnit.TotalHitChance.ToString() + "%</color>";
		}
		else if(Logic.Inst.SelectedUnit.TotalHitChance < Logic.Inst.SelectedUnit.hitChance){
			predictionStats[0].hit.text = "<color=red>" + Logic.Inst.SelectedUnit.TotalHitChance.ToString() + "%</color>";
		}
		else{
			predictionStats[0].hit.text = Logic.Inst.SelectedUnit.TotalHitChance.ToString();
		}
		
		if(Logic.Inst.SelectedUnit.TotalDodgeChance > Logic.Inst.SelectedUnit.TotalDodgeChance){
			predictionStats[0].dodge.text = "<color=green>" + Logic.Inst.SelectedUnit.TotalDodgeChance.ToString() + "%</color>";
		}
		else if(Logic.Inst.SelectedUnit.TotalDodgeChance < Logic.Inst.SelectedUnit.dodgeChance){
			predictionStats[0].dodge.text = "<color=red>" + Logic.Inst.SelectedUnit.TotalDodgeChance.ToString() + "%</color>";
		}
		else{
			predictionStats[0].dodge.text = Logic.Inst.SelectedUnit.TotalDodgeChance.ToString();
		}
				
		// update Defender Stats
		predictionStats[1].unitName.text = hoverUnit.name;
		predictionStats[1].hp.text = hoverUnit.CurrentHitpoints.ToString() + " > " +
			(hoverUnit.CurrentHitpoints - DamageDealt(0)) + " (<color=red>-" + DamageDealt(0) + "</color>)";

		if(hoverUnit.TotalAttack > hoverUnit.attack){
			predictionStats[1].atk.text = "<color=green>" + hoverUnit.TotalAttack.ToString() + "</color>";
		}
		else if(hoverUnit.TotalAttack < hoverUnit.attack){
			predictionStats[1].atk.text = "<color=red>" + hoverUnit.TotalAttack.ToString() + "</color>";
		}
		else{
			predictionStats[1].atk.text = hoverUnit.TotalAttack.ToString();
		}
		
		if(hoverUnit.TotalDefense > hoverUnit.defense){
			predictionStats[1].def.text = "<color=green>" + hoverUnit.TotalDefense.ToString() + "</color>";
		}
		else if(hoverUnit.TotalDefense < hoverUnit.defense){
			predictionStats[1].def.text = "<color=red>" + hoverUnit.ToString() + "</color>";
		}
		else{
			predictionStats[1].def.text = hoverUnit.TotalDefense.ToString();
		}
		
		if(hoverUnit.TotalHitChance > hoverUnit.hitChance){
			predictionStats[1].hit.text = "<color=green>" + hoverUnit.TotalHitChance.ToString() + "%</color>";
		}
		else if(hoverUnit.TotalHitChance < hoverUnit.hitChance){
			predictionStats[1].hit.text = "<color=red>" + hoverUnit.TotalHitChance.ToString() + "%</color>";
		}
		else{
			predictionStats[1].hit.text = hoverUnit.TotalHitChance.ToString();
		}
		
		if(hoverUnit.TotalDodgeChance > hoverUnit.TotalDodgeChance){
			predictionStats[1].dodge.text = "<color=green>" + hoverUnit.TotalDodgeChance.ToString() + "%</color>";
		}
		else if(hoverUnit.TotalDodgeChance < hoverUnit.dodgeChance){
			predictionStats[1].dodge.text = "<color=red>" + hoverUnit.ToString() + "%</color>";
		}
		else{
			predictionStats[1].dodge.text = hoverUnit.TotalDodgeChance.ToString();
		}
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
