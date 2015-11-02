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

	void Start () {
		hero = this.gameObject.GetComponent<Unit>();
		origActive1Cost = active1.cost;
		origActive2Cost = active2.cost;

		foreach(Buff buff in active1.buffs){
			origActive1Strengths.Add(buff.strength);
		}

		foreach(Buff buff in active2.buffs){
			origActive2Strengths.Add(buff.strength);
		}
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
				Debug.Log("Entering Target Mode!");
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
				foreach(Target targ in targets){
					if(targ.type == TargetType.AoE){
						currentAbility.ApplyBuffAoE(targ.Index);
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

		foreach(Buff buff in active1.buffs){
			int index;
			index = active1.buffs.IndexOf(buff);
			buff.strength = Mathf.RoundToInt(origActive1Strengths[index] * (1 + (abilityBonus * (4 - hero.Owner.capturedAltars.Count))));
		}

		foreach(Buff buff in active2.buffs){
			int index;
			index = active2.buffs.IndexOf(buff);
			buff.strength = Mathf.RoundToInt(origActive2Strengths[index] * (1 + (abilityBonus * (4 - hero.Owner.capturedAltars.Count))));
		}
	}
}
