using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero : MonoBehaviour {

	public HeroType type;
	public Skill passive;
	public Skill active1;
	public Skill active2;
	public int currentAbility;
	public int currentRange; 
	public AbilityStage currentStage = AbilityStage.Done;
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

//		if(Input.GetKeyDown(KeyCode.Alpha3) && gameObject.GetComponent<Unit>().Owner == Logic.Inst.Players[1]){
//			if(active1.target == TargetType.Single){
//				Logic.Inst.HighlightAbilityRange(active1,hero);
//			}
//			ActivateAbility1();
//		}
//
//		if(Input.GetKeyDown(KeyCode.Alpha3) && gameObject.GetComponent<Unit>().Owner == Logic.Inst.Players[1]){
//			if(active2.target == TargetType.Single){
//				Logic.Inst.HighlightAbilityRange(active2,hero);
//			}
//			ActivateAbility2();
//		}
	}

	public void ReceiveTarget(Unit unit, Tile tile){
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
	}
//
	void ActivateAbility1(){
		if(hero.Owner.Faith >= active1.cost){
			if(active1.target == AimType.Single){
				targets.Clear();
				Logic.Inst.HighlightAbilityRange(active1,hero);
//				currentStage = AbilityStage.GetUnit;
				Logic.Inst.gamePhase = GamePhase.TargetPhase;
				Debug.Log("Entering Target Mode!");
				currentAbility = 1;
				currentRange = active1.castRange;
			}	
		}
	}

	void CastAbility1(){
		if(active1.target == AimType.Single){
			active1.ApplyBuffSingle(targets[0].Index);
		}
		else if(active1.target == AimType.All){
			if(active1.hitFoe){
				active1.ApplyBuffAll((Logic.Inst.currentPlayer-1) % 2);
			}
			else{
				active1.ApplyBuffAll(Logic.Inst.currentPlayer);
			}
		}
		else if(active1.target == AimType.SelfAoE){
			active1.ApplyBuffAoE(hero.Index);
		}
		else{
			foreach(Target targ in targets){
				if(targ.type == TargetType.AoE){
					active1.ApplyBuffAoE(targets[0].Index);
				}
			}
		}
	}
//
	void ActivateAbility2(){
		if(hero.Owner.Faith >= active2.cost){	
			if(active2.target == AimType.Single){
				Logic.Inst.HighlightAbilityRange(active2,hero);
			}
//			if (active2.target == TargetType.All || active2.target == TargetType.AoE) {
//				active2.Activate (gameObject.GetComponent<Unit> ());
//			} 
//			else if (active2.target == TargetType.Single) {
//				currentStage = AbilityStage.GetUnit;
//				Logic.Inst.gamePhase = GamePhase.TargetPhase;
//				Debug.Log ("Entering Target Mode!");
//				currentAbility = 2;
//				currentRange = active2.range;
//			}

//			if (type == HeroType.Skadi && hero.Owner.wrathMode) {
//				skadiWrathCheck = true;
//				}
		}
	}
//
//	public void ApplyPassive(){
//		if(passive.passive == PassiveType.OneShot){
//			passive.Activate(gameObject.GetComponent<Unit>());
//		}
//	}
}
