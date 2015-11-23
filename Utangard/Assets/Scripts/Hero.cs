using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Hero : MonoBehaviour {

	public HeroType type;
	public Skill passive;
	public Skill active1;
	public Skill active2;
	private int origActive1Cost;
	private int origActive2Cost;
	private List<int> origActive1Strengths = new List<int>();
	private List<int> origActive2Strengths = new List<int>();
	public Skill currentAbility;
	public int currentRange; 
	public int currentStage = 0;
	public float abilityBonus, costIncrease;
	public Unit hero;
	private Unit target;
	private Tile teleLocation;
	public List<Target> targets  = new List<Target>();
	private String[] tooltipText = new string[3]; // 0 passive, 1 Ability1, 2 ability2

	void Start () {
		hero = this.gameObject.GetComponent<Unit>();
		origActive1Cost = active1.cost;
		origActive2Cost = active2.cost;

		for(int i = 0; i < active1.buffs.Count; i ++){
			origActive1Strengths.Add(active1.buffs[i].strength);
			active1.buffs[i].skillID = active1.ID;
		}

		for(int j = 0; j < active2.buffs.Count; j++){
			origActive2Strengths.Add(active2.buffs[j].strength);
			active2.buffs[j].skillID = active2.ID;
		}

		for(int k = 0; k < passive.buffs.Count; k++){
			passive.buffs[k].skillID = passive.ID;
		}

		SetToolTips();
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1) && gameObject.GetComponent<Unit>().Owner == Logic.Inst.Players[0]){
			ActivateAbility1();
		}

		if(Input.GetKeyDown(KeyCode.Alpha2) && gameObject.GetComponent<Unit>().Owner == Logic.Inst.Players[0]){
			ActivateAbility2();
		}

		if (Input.GetKeyDown (KeyCode.F)) {
			hero.Owner.Faith += 1000000;
		}
	}

	public void ActivateAbility1(){
		if(hero.Owner.Faith >= active1.cost){
			if(active1.targets.Count > 0){
				active1.targets[0].origin = hero.Index;
			}
			targets.Clear();
			currentStage = 0;
			currentAbility = active1;
			currentRange = active1.castRange;
			if(active1.target == AimType.Single || active1.target == AimType.TargetAoE){
				Logic.Inst.HighlightAbilityRange(active1,hero.Index);
				Logic.Inst.gamePhase = GamePhase.TargetPhase;
			}
			else if(active1.target == AimType.All || active1.target == AimType.SelfAoE){
				CastAbility();
			}
		}
	}

	public void CastAbility(){
		if(currentAbility.abilityType == AbilityType.Buff){
			if(currentAbility.target == AimType.Single){
				currentAbility.ApplyBuffSingle(targets[0].Index);
			}
			else if(currentAbility.target == AimType.All){
				if(currentAbility.hitFoe){
					currentAbility.ApplyBuffAll((Logic.Inst.currentPlayer-1) % 2);
				}
				else{
					currentAbility.ApplyBuffAll(Logic.Inst.currentPlayer);
				}
			}
			else if(currentAbility.target == AimType.SelfAoE){
				print (gameObject.name);
				currentAbility.ApplyBuffAoE(hero.Index);
			}
			else{
				for(int i = 0; i < targets.Count; i++){
					if(targets[i].type == TargetType.AoE){
						currentAbility.ApplyBuffAoE(targets[i].Index);
					}
				}
			}
		}
		else if (currentAbility.abilityType == AbilityType.Teleport){	//Teleport Unit
			targets[0].unit.MoveTowardsTile(Logic.Inst.Grid.TileAt(targets[1].Index));

			if(hero.Owner.wrathMode){
				currentAbility.ApplyBuffAoE(targets[1].Index);
			}
		}

		hero.Owner.Faith -= currentAbility.cost;
		Logic.Inst.ClearHighlightedTiles();

		if(currentAbility.ID == active1.ID){
			gameObject.GetComponent<Unit>().ChangeAnim(5);
		}
		else if(currentAbility.ID == active2.ID){
			gameObject.GetComponent<Unit>().ChangeAnim(6);
		}
	}

	public void ActivateAbility2(){
		if(hero.Owner.Faith >= active2.cost){	
			if(active2.targets.Count > 0){
				active2.targets[0].origin = hero.Index;
			}
			targets.Clear();
			currentStage = 0;
			currentAbility = active2;
			currentRange = active2.castRange;
			if(active2.target == AimType.Single || active2.target == AimType.TargetAoE){
				Logic.Inst.HighlightAbilityRange(active2,hero.Index);
				Logic.Inst.gamePhase = GamePhase.TargetPhase;
			}
			else if(active2.target == AimType.All || active2.target == AimType.SelfAoE){
				CastAbility();
			}
		}
	}

	public void ApplyPassive(){
		if(passive.targets.Count > 0){
			passive.targets[0].origin = hero.Index;
		}
		targets.Clear();
		currentStage = 0;
		currentAbility = passive;
		currentRange = passive.castRange;

		if(passive.passive == PassiveType.OneShotAoE || passive.passive == PassiveType.PersitentAoE){
			CastAbility();
		}
	}

	public void CheckTarget(Tile tile){
		List<Tile> inRange = new List<Tile>();

		inRange = Logic.Inst.Grid.TilesInRange(currentAbility.targets[currentStage].origin,currentAbility.castRange);

		if(inRange.Contains(tile) && targets.Count < currentAbility.targets.Count){
			//Oh god this if is so long...Designed to make sure you can't put the wrong kind of targets on the list. Eg. Makes sure you're targeting a tile with a unit, if the ability hurts a specific unit.
			if((currentAbility.targets[currentStage].needsSpace && !tile.OccupyingUnit) || (currentAbility.targets[currentStage].needsUnit && tile.OccupyingUnit) || (!currentAbility.targets[currentStage].needsSpace && !currentAbility.targets[currentStage].needsUnit)){
				targets.Add(new Target(tile.OccupyingUnit,tile.index,currentAbility.targets[currentStage].type,false,false));
				currentAbility.targets[currentStage].unit = tile.OccupyingUnit;

				if(targets.Count < currentAbility.targets.Count){
					currentStage++;
				}

				Logic.Inst.ClearHighlightedTiles();

				if(currentStage > 0 && currentAbility.targets[currentStage-1].unit){
					currentAbility.targets[currentStage].origin = currentAbility.targets[currentStage-1].unit.Index;
					Logic.Inst.HighlightAbilityRange(currentAbility,currentAbility.targets[currentStage].origin);
				}
			}
		}
	}

	public void CalcBuffStrength(){
		active1.cost = Mathf.RoundToInt(origActive1Cost * (1 + (costIncrease * (4 - hero.Owner.capturedAltars.Count))));
		active2.cost = Mathf.RoundToInt(origActive2Cost * (1 + (costIncrease * (4 - hero.Owner.capturedAltars.Count))));

		for(int i = 0; i < active1.buffs.Count; i++){
			int index;
			index = active1.buffs.IndexOf(active1.buffs[i]);
			active1.buffs[i].strength = Mathf.RoundToInt(origActive1Strengths[index] * (1 + (abilityBonus * ((Logic.Inst.numAltars/2) - hero.Owner.capturedAltars.Count))));
		}

		for(int j = 0; j < active2.buffs.Count; j++){
			int index;
			index = active2.buffs.IndexOf(active2.buffs[j]);
			active2.buffs[j].strength = Mathf.RoundToInt(origActive2Strengths[index] * (1 + (abilityBonus * ((Logic.Inst.numAltars/2) - hero.Owner.capturedAltars.Count))));
		}
	}

	void SetToolTips(){
		switch(type){
		case HeroType.Eir:
			tooltipText[0] = 
				"<size=20><b>Nine Maiden's Blessings</b></size> -" +"\n"+
					"<size=17>Restore a small amount of <color=lime><i>Health</i></color> to units adjacent to <color=yellow><i>Eir</i></color>." +"\n"+
					"</size><color=yellow>Target: Friendly</color>" +"\n"+
					"<color=yellow>Radius: 1 tiles</color>" +"\n"+
					"<color=lime>+" + passive.buffs[0].strength + " Health</color>";
			tooltipText[1] = 
				"<size=20><b>Rejuvenate</b></size> -" +"\n"+
					"<size=17>Restore a massive amount of <color=lime><i>Health</i></color> to a friendly unit within 3 tiles.</size>" +"\n"+
					"<color=cyan>Cost: " + active1.cost + " Faith</color>" +"\n"+
					"<color=yellow>Target: Friendly</color>" +"\n"+
					"<color=yellow>Range: 3 tiles</color>" +"\n"+
					"<color=lime>+" + active1.buffs[0].strength + " Health</color>" +"\n"+
					"<color=lime>+" + active1.buffs[1].strength + " Defence</color> (<color=red>Wrath</color>)" +"\n"+
					"<color=yellow>Duration: 1 turns</color> (<color=red>Wrath</color>)";
			tooltipText[2] =
				"<size=20><b>Goddess' Light</b></size> -" +"\n"+
					"<size=17>Restore a small amount of <color=lime><i>Health</i></color> to every friendly unit on the field.</size>" +"\n"+
					"<color=cyan>Cost: " + active2.cost + " Faith</color>" +"\n"+
					"<color=yellow>Target: Friendly</color>" +"\n"+
					"<color=yellow>Range: All tiles</color>" +"\n"+
					"<color=lime>+" + active2.buffs[0].strength + " Health</color>" +"\n"+
					"<color=lime>+" + active2.buffs[1].strength + " Attack</color> (<color=red>Wrath</color>)" +"\n"+
					"<color=lime>+" + active2.buffs[2].strength + " Defence</color> (<color=red>Wrath</color>)" +"\n"+
					"<color=yellow>Duration: 2 turns</color> (<color=red>Wrath</color>)";
			break;
		case HeroType.Heimdal:
			tooltipText[0] =
				"<size=20><b>Innangard</b></size> -" + "\n"+
					"<size=17>All units gain a small amount of <color=orange><i>defence</i></color> for each friendly unit adjacent to them.</size>" +"\n"+
					"<color=yellow>Target: Friendly</color>" +"\n"+
					"<color=yellow>Radius: 1 tiles</color>" +"\n"+
					"<color=lime>+" + passive.buffs[0].strength + "Defence</color>";
			tooltipText[1] =
				"<size=20><b>Bifrost Advance</b></size> -" +"\n"+
					"<size=17>Move a Friendly unit within 2 tiles of the Heimdall's hero, to an empty tile within 2 tiles of the unit.</size>"+"\n"+
					"<color=cyan>Cost: " + active1.cost + "Faith</color>" +"\n"+
					"<color=yellow>Target: Friendly</color>, (<color=red>Wrath</color>) <color=yellow>Enemy</color>"+"\n"+
					"<color=yellow>Range: 2 tiles (Heimdall), 2 tiles (Unit)</color>"+"\n"+
					"<color=lime>Move a unit up to 2 tiles</color>"+"\n"+
					"<color=red>" + active1.buffs[0].strength + "defence</color> to adjacent Enemies (<color=red>Wrath</color>)";
			tooltipText[2] = 
				"<size=20><b>Stalwart Defence</b></size> -" +"\n"+
					"<size=17>Improve the <color=orange><i>Defence</i></color> of friendly units near Heimdall's Hero.</size>"+"\n"+
					"<color=cyan>Cost: " + active2.cost + " Faith</color>" +"\n"+
					"<color=yellow>Target: Friendly</color>"+"\n"+
					"<color=yellow>Radius: 2 tiles</color>"+"\n"+
					"<color=yellow>Duration: 2 turns</color>"+"\n"+
					"<color=lime>+" + active2.buffs[0].strength + " Defence</color>"+"\n"+
					"<color=lime>+" + active2.buffs[1].strength + " Hit Chance</color> (<color=red>Wrath</color>);";
			break;
		case HeroType.Skadi:
			tooltipText[0] = 
				"<size=20><b>Grace Of The Huntress</b></size> -"+"\n"+
					"<size=17>Units no longer receive a movement penalty when traversing <color=cyan><i>snow</i></color>.</size>"+"\n"+
					"<color=yellow>Target: Friendly</color>"+"\n"+
					"<color=yellow>Range: All Tiles</color>";
			tooltipText[1] =
				"<size=20><b>Thrill Of The Hunt</b></size> -"+"\n"+
					"<size=17>All spearmen gain improved <color=orange><i>Attack Range</i></color> for a turn.</size>"+"\n"+
					"<color=cyan>Cost: " + active1.cost + " Faith</color>" +"\n"+
					"<color=yellow>Target: Friendly Spearmen</color>"+"\n"+
					"<color=yellow>Range: All tiles</color>"+"\n"+
					"<color=yellow>Duration: 1 Turn</color>"+"\n"+
					"<color=lime>+" + active1.buffs[0].strength + " Range</color>"+"\n"+
					"<color=lime>+" + active1.buffs[1].strength + " Hit Chance</color> (<color=red>Wrath</color>)";
			tooltipText[2] =
				"<size=20><b>In For The Kill</b></size> -"+"\n"+
					"<size=17>All spearmen gain <color=orange><i>Attack</i></color> but lose <color=orange><i>Attack Range</i></color> for a turn.</size>"+"\n"+
					"<color=cyan>Cost: " + active2.cost + " Faith</color>" +"\n"+
					"<color=yellow>Target: Friendly Spearmen</color>"+"\n"+
					"<color=yellow>Range: All tiles</color>"+"\n"+
					"<color=yellow>Duration: 1 Turn</color>"+"\n"+
					"<color=lime>+" + active2.buffs[0].strength + " Attack</color>"+"\n"+
					"<color=red>" + active2.buffs[1].strength + " Range</color>"+"\n"+
					"<color=lime>+" + active2.buffs[2].strength + " Hit Chance</color> (<color=red>Wrath</color>)";
			break;
		case HeroType.Thor:
			tooltipText[0] =
				"<size=20><b>Inspire</b></size> -"+ "\n"+
					"<size=17>Units nearby Thor's Hero receive bonus <color=orange><i>Attack</i></color>.</size>"+ "\n"+
					"<color=yellow>Target: Friendly</color>"+ "\n"+
					"<color=yellow>Range: 2 tiles</color>"+ "\n"+
					"<color=lime>+" + passive.buffs[0].strength + " Attack</color>";

			tooltipText[1] =
				"<size=20><b>Thor's Wrath</b></size> -"+ "\n"+
					"<size=17>Strike an enemy with lightning for massive <color=red><i>Damage</i></color>. (<color=yellow>Ignores <i>Defence</i></color>)</size>"+ "\n"+
					"<color=cyan>Cost: " + active1.cost + " Faith</color>" + "\n"+
					"<color=yellow>Target: Enemy</color>"+ "\n"+
					"<color=yellow>Range: 3 tiles</color>"+ "\n"+
					"<color=red>-" + active1.buffs[0].strength + " Health</color>"+ "\n"+
					"<color=red>-" + active1.buffs[1].strength + " Health</color> to adjacent enemies(<color=red>Wrath</color>)";

			tooltipText[2] =
				"<size=20><b>Odin's Beard</b></size> -"+ "\n"+
					"<size=17>All friendly units gain a moderate boost <color=orange><i>Defence</i></color> for 2 turns. Promotes healthy <color=lime><i>beard growth</i></color>.</size>"+"\n"+
					"<color=cyan>Cost: " + active2.cost + " Faith</color>" +"\n"+
					"<color=yellow>Target: Friendly</color>" + "\n"+
					"<color=yellow>Range: All tiles</color>"+"\n"+
					"<color=yellow>Duration: 2 turns</color>"+"\n"+
					"<color=lime>+" + active2.buffs[0].strength + " Defence</color>"+"\n"+
					"<color=lime>+" + active2.buffs[1].strength + " Movement</color> (<color=red>Wrath</color>)";
			break;
		case HeroType.Sam:
			tooltipText[0] = "Run";
			tooltipText[1] = "No, really... Run";
			tooltipText[2] = "FUCKING RUN!!";
			break;
		}
	}

	public string GetPassiveText(){
		SetToolTips();
		return tooltipText[0];
	}
	public string GetAbility1Text(){
		SetToolTips();
		return tooltipText[1];
	}
	public string GetAbility2Text(){
		SetToolTips();
		return tooltipText[2];
	}
}
