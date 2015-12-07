using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattlePredictionUI : MonoBehaviour {

	[System.Serializable]
	public struct PredictionText{
		public Image icon;
		public Text damageText;
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
				if(Logic.Inst.SelectedUnit != null){
					UpdateStats();
					UpdateBreakdown();
				}
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

        ChangeIcons();
		// 0 == attacker, 1 == defender
		predictText[0].chanceText.text = ChanceToHit(1).ToString() + "%";
		predictText[1].chanceText.text = ChanceToHit(0).ToString() + "%";

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
			//          Mathf.RoundToInt(atk.TotalAttack * (1 - ((defNumA * def.TotalDefense)/(defNumB + def.TotalDefense))));
			return (int)Mathf.RoundToInt((Logic.Inst.SelectedUnit.TotalAttack * (1 -((magicX * hoverUnit.TotalDefense)/(magicY + hoverUnit.TotalDefense)))));
		}
		else{
			return (int)Mathf.RoundToInt((hoverUnit.TotalAttack * (1 -((magicX * Logic.Inst.SelectedUnit.TotalDefense)/(magicY + Logic.Inst.SelectedUnit.TotalDefense)))));
		}
	}

	void UpdateStats(){
		// update Attacker stats
		if(Logic.Inst.SelectedUnit != null){
			predictionStats[0].unitName.text = Logic.Inst.SelectedUnit.name;
			predictionStats[0].hp.text = Logic.Inst.SelectedUnit.CurrentHitpoints.ToString();
			predictText[0].damageText.text = DamageDealt(0).ToString();

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
		}
				
		// update Defender Stats
		if(hoverUnit != null){
			predictionStats[1].unitName.text = hoverUnit.name;
			predictionStats[1].hp.text = hoverUnit.CurrentHitpoints.ToString();
			predictText[1].damageText.text = DamageDealt(1).ToString();

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
		}
	}

    void ChangeIcons()
    {
        switch (Logic.Inst.SelectedUnit.Owner.hero.type)
        {       
            case HeroType.Eir:
                switch (Logic.Inst.SelectedUnit.type) 
                {
                    case UnitType.Axemen:
                        predictText[0].icon.sprite = GUIManager.inst.unitPortraits[0].axe;
                        break;
                    case UnitType.Spearman:
                        predictText[0].icon.sprite = GUIManager.inst.unitPortraits[0].spear;
                        break;
                    case UnitType.Swordsmen:
                        predictText[0].icon.sprite = GUIManager.inst.unitPortraits[0].sword;
                        break;
                    case UnitType.Hero:
                        predictText[0].icon.sprite = GUIManager.inst.heroPortraits[0];
                        break;
                }
                break;
            case HeroType.Heimdall:
                switch (Logic.Inst.SelectedUnit.type)
                {
                    case UnitType.Axemen:
                        predictText[0].icon.sprite = GUIManager.inst.unitPortraits[1].axe;
                        break;
                    case UnitType.Spearman:
                        predictText[0].icon.sprite = GUIManager.inst.unitPortraits[1].spear;
                        break;
                    case UnitType.Swordsmen:
                        predictText[0].icon.sprite = GUIManager.inst.unitPortraits[1].sword;
                        break;
                    case UnitType.Hero:
                        predictText[0].icon.sprite = GUIManager.inst.heroPortraits[1];
                        break;
                }
                break;
            case HeroType.Skadi:
                switch (Logic.Inst.SelectedUnit.type)
                {
                    case UnitType.Axemen:
                        predictText[0].icon.sprite = GUIManager.inst.unitPortraits[2].axe;
                        break;
                    case UnitType.Spearman:
                        predictText[0].icon.sprite = GUIManager.inst.unitPortraits[2].spear;
                        break;
                    case UnitType.Swordsmen:
                        predictText[0].icon.sprite = GUIManager.inst.unitPortraits[2].sword;
                        break;
                    case UnitType.Hero:
                        predictText[0].icon.sprite = GUIManager.inst.heroPortraits[2];
                        break;
                }
                break;
            case HeroType.Thor:
                switch (Logic.Inst.SelectedUnit.type)
                {
                    case UnitType.Axemen:
                        predictText[0].icon.sprite = GUIManager.inst.unitPortraits[3].axe;
                        break;
                    case UnitType.Spearman:
                        predictText[0].icon.sprite = GUIManager.inst.unitPortraits[3].spear;
                        break;
                    case UnitType.Swordsmen:
                        predictText[0].icon.sprite = GUIManager.inst.unitPortraits[3].sword;
                        break;
                    case UnitType.Hero:
                        predictText[0].icon.sprite = GUIManager.inst.heroPortraits[3];
                        break;
                }
                break;      
        }


        switch (hoverUnit.Owner.hero.type)
        {
            case HeroType.Eir:
                switch (hoverUnit.type)
                {
                    case UnitType.Axemen:
                        predictText[1].icon.sprite = GUIManager.inst.unitPortraits[0].axe;
                        break;
                    case UnitType.Spearman:
                        predictText[1].icon.sprite = GUIManager.inst.unitPortraits[0].spear;
                        break;
                    case UnitType.Swordsmen:
                        predictText[1].icon.sprite = GUIManager.inst.unitPortraits[0].sword;
                        break;
                    case UnitType.Hero:
                        predictText[1].icon.sprite = GUIManager.inst.heroPortraits[0];
                        break;
                }
                break;
            case HeroType.Heimdall:
                switch (hoverUnit.type)
                {
                    case UnitType.Axemen:
                        predictText[1].icon.sprite = GUIManager.inst.unitPortraits[1].axe;
                        break;
                    case UnitType.Spearman:
                        predictText[1].icon.sprite = GUIManager.inst.unitPortraits[1].spear;
                        break;
                    case UnitType.Swordsmen:
                        predictText[1].icon.sprite = GUIManager.inst.unitPortraits[1].sword;
                        break;
                    case UnitType.Hero:
                        predictText[1].icon.sprite = GUIManager.inst.heroPortraits[1];
                        break;
                }
                break;
            case HeroType.Skadi:
                switch (hoverUnit.type)
                {
                    case UnitType.Axemen:
                        predictText[1].icon.sprite = GUIManager.inst.unitPortraits[2].axe;
                        break;
                    case UnitType.Spearman:
                        predictText[1].icon.sprite = GUIManager.inst.unitPortraits[2].spear;
                        break;
                    case UnitType.Swordsmen:
                        predictText[1].icon.sprite = GUIManager.inst.unitPortraits[2].sword;
                        break;
                    case UnitType.Hero:
                        predictText[1].icon.sprite = GUIManager.inst.heroPortraits[2];
                        break;
                }
                break;
            case HeroType.Thor:
                switch (hoverUnit.type)
                {
                    case UnitType.Axemen:
                        predictText[1].icon.sprite = GUIManager.inst.unitPortraits[3].axe;
                        break;
                    case UnitType.Spearman:
                        predictText[1].icon.sprite = GUIManager.inst.unitPortraits[3].spear;
                        break;
                    case UnitType.Swordsmen:
                        predictText[1].icon.sprite = GUIManager.inst.unitPortraits[3].sword;
                        break;
                    case UnitType.Hero:
                        predictText[1].icon.sprite = GUIManager.inst.heroPortraits[3];
                        break;
                }
                break;
        }


    }

	void ClearStats(){
		predictionStats[0].unitName.text = "";
		predictionStats[0].hp.text = "";
		predictionStats[0].atk.text = "";
		predictionStats[0].def.text = "";


		predictionStats[1].unitName.text = "";
		predictionStats[1].hp.text = "";
		predictionStats[1].atk.text = "";
		predictionStats[1].def.text = "";

	}
}
