using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Hero : MonoBehaviour {

	public HeroType type;
	public Skill passive;
	public Skill active1;
	public Skill active2;
	public Skill currentAbility;
	public int currentRange; 
	public int currentStage = 0;
	//public AbilityStage currentStage = AbilityStage.Done;
	public Unit hero;
	public bool skadiWrathCheck = false;
	private Unit target;
	private Tile teleLocation;
	public List<Target> targets  = new List<Target>();

	void Start () {
		hero = gameObject.GetComponent<Unit>();
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1) && gameObject.GetComponent<Unit>().Owner == Logic.Inst.Players[0]){
			ActivateAbility1();
		}

		if(Input.GetKeyDown(KeyCode.Alpha2) && gameObject.GetComponent<Unit>().Owner == Logic.Inst.Players[0]){
			ActivateAbility2();
		}
	}

//	public void ReceiveTarget(Unit unit, Tile tile){
//		List<Tile> inRange = new List<Tile>();
//		bool ready = false;
//
//		if(currentStage == AbilityStage.GetUnit){
//			print("Target Received!");
//			inRange = Logic.Inst.Grid.AbilityRange(gameObject.GetComponent<Unit>().Index,currentRange);
//
//			if(inRange.Contains(Logic.Inst.Grid.GetTile(unit.Index))){
//				if(currentAbility == 1){
//					active1.Activate(unit);
//					if(active1.stages <= 1){
//						currentStage = AbilityStage.Done;
//					}
//				}
//				else{
//					active2.Activate(unit);
//					if(active2.stages <= 1){
//						currentStage = AbilityStage.Done;
//					}
//				}
//
//				target = unit;
//
//				ready = true;
//			}
//
//			else{
//				target = null;
//				ready = false;
//			}
//		}
//		else{
//			inRange = Logic.Inst.Grid.AbilityRange(target.Index,currentRange);
//
//			if(inRange.Contains(Logic.Inst.Grid.GetTile(tile.Index)) && !tile.OccupyngUnit){
//				if(currentAbility == 1){
//					active1.TeleportUnit(target,tile);
//				}
//				else{
//					active2.TeleportUnit(target,tile);
//				}
//				currentStage = AbilityStage.Done;
//			}
//		}
//
//		if(currentStage == AbilityStage.Done){
//			Logic.Inst.gamePhase = GamePhase.CombatPhase;
//			Logic.Inst.ClearHighlightedTiles();
//			target = null;
//			print("Back to combat!");
//		}
//		else if(ready){
//			currentStage = currentStage = AbilityStage.GetLocation;
//		}
//	}

	public void ActivateAbility1(){
		if(hero.Owner.Faith >= active1.cost){
			active1.targets[0].origin = hero.Index;
			targets.Clear();
			if(active1.target == AimType.Single || active1.target == AimType.TargetAoE){
				currentStage = 0;
				currentAbility = active1;
				currentRange = active1.castRange;
				Logic.Inst.HighlightAbilityRange(active1,hero);
				Logic.Inst.gamePhase = GamePhase.TargetPhase;
				Debug.Log("Entering Target Mode!");
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

		hero.Owner.Faith -= currentAbility.cost;
	}

	void ActivateAbility2(){
		if(hero.Owner.Faith >= active2.cost){	
			active2.targets[0].origin = hero.Index;
			targets.Clear();
			if(active2.target == AimType.Single || active2.target == AimType.TargetAoE){
				currentStage = 0;
				currentAbility = active2;
				currentRange = active2.castRange;
				Logic.Inst.HighlightAbilityRange(active2,hero);
				Logic.Inst.gamePhase = GamePhase.TargetPhase;
				Debug.Log("Entering Target Mode!");
			}	
		}
	}

	public void ApplyPassive(){
		if(passive.passive == PassiveType.OneShotAoE || passive.passive == PassiveType.PersitentAoE){
			if(passive.hitFoe){
				passive.ApplyBuffAll((Logic.Inst.currentPlayer-1) % 2);
			}
			else{
				passive.ApplyBuffAll(Logic.Inst.currentPlayer);
			}
		}
	}

	public void CheckTarget(Tile tile){
		List<Tile> inRange = new List<Tile>();

		inRange = Logic.Inst.Grid.AbilityRange(currentAbility.targets[currentStage].origin,currentAbility.castRange);

		if(inRange.Contains(tile)){
			//Oh god this if is so long...Designed to make sure you can't put the wrong kind of targets on the list. Eg. Makes sure you're targeting a tile with a unit, if the ability hurts a specific unit.
			if((currentAbility.targets[currentStage].needsSpace && !tile.OccupyngUnit) || (currentAbility.targets[currentStage].needsUnit && tile.OccupyngUnit) || (!currentAbility.targets[currentStage].needsSpace && !currentAbility.targets[currentStage].needsUnit)){
				targets.Add(null);
				targets[currentStage].unit = tile.OccupyngUnit;
				targets[currentStage].Index = tile.Index;
				targets[currentStage].type = currentAbility.targets[currentStage].type;
				currentStage++;

				try{
					currentAbility.targets[currentStage].origin = currentAbility.targets[currentStage-1].unit.Index;
				}
				catch (Exception e){

				}
			}
		}
	}
}
