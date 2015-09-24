using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero : MonoBehaviour {

	public HeroType type;
	public Ability passive;
	public Ability active1;
	public Ability active2;
	public int currentAbility;
	public int currentRange; 
	public AbilityStage currentStage = AbilityStage.Done;
	public Unit hero;
	public bool skadiWrathCheck = false;
	private Unit target;
	private Tile teleLocation;

	void Start () {
		//gameObject.GetComponent<Unit>().Owner = Logic.Inst.Players[0];	//This is purely for testing. Be sure to remove later.
		//Logic.Inst.Players[0].army.Add(gameObject.GetComponent<Unit>());	//Also for testing.
		hero = gameObject.GetComponent<Unit>();
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1) && gameObject.GetComponent<Unit>().Owner == Logic.Inst.Players[0]){
			if(active1.target == TargetType.Single){
				Logic.Inst.HighlightAbilityRange(active1,hero);
			}
			ActivateAbility1();
		}

		if(Input.GetKeyDown(KeyCode.Alpha2) && gameObject.GetComponent<Unit>().Owner == Logic.Inst.Players[0]){
			if(active2.target == TargetType.Single){
				Logic.Inst.HighlightAbilityRange(active2,hero);
			}
			ActivateAbility2();
		}

		if(Input.GetKeyDown(KeyCode.Alpha3) && gameObject.GetComponent<Unit>().Owner == Logic.Inst.Players[1]){
			if(active1.target == TargetType.Single){
				Logic.Inst.HighlightAbilityRange(active1,hero);
			}
			ActivateAbility1();
		}

		if(Input.GetKeyDown(KeyCode.Alpha3) && gameObject.GetComponent<Unit>().Owner == Logic.Inst.Players[1]){
			if(active2.target == TargetType.Single){
				Logic.Inst.HighlightAbilityRange(active2,hero);
			}
			ActivateAbility2();
		}
	}

	public void ReceiveTarget(Unit unit, Tile tile){
		List<Tile> inRange = new List<Tile>();
		bool ready = false;

		if(currentStage == AbilityStage.GetUnit){
			print("Target Received!");
			inRange = Logic.Inst.Grid.AbilityRange(gameObject.GetComponent<Unit>().Index,currentRange);

			if(inRange.Contains(Logic.Inst.Grid.GetTile(unit.Index))){
				if(currentAbility == 1){
					active1.Activate(unit);
					if(active1.stages <= 1){
						currentStage = AbilityStage.Done;
					}
				}
				else{
					active2.Activate(unit);
					if(active2.stages <= 1){
						currentStage = AbilityStage.Done;
					}
				}

				target = unit;

				ready = true;
			}

			else{
				target = null;
				ready = false;
			}
		}
		else{
			inRange = Logic.Inst.Grid.AbilityRange(target.Index,currentRange);

			if(inRange.Contains(Logic.Inst.Grid.GetTile(tile.Index)) && !tile.OccupyngUnit){
				if(currentAbility == 1){
					active1.TeleportUnit(target,tile);
				}
				else{
					active2.TeleportUnit(target,tile);
				}
				currentStage = AbilityStage.Done;
			}
		}

		if(currentStage == AbilityStage.Done){
			Logic.Inst.gamePhase = GamePhase.CombatPhase;
			Logic.Inst.ClearHighlightedTiles();
			target = null;
			print("Back to combat!");
		}
		else if(ready){
			currentStage = currentStage = AbilityStage.GetLocation;
		}
	}

	void ActivateAbility1(){
		if(active1.target == TargetType.All || active1.target == TargetType.AoE){
			active1.Activate(gameObject.GetComponent<Unit>());
		}
		else if(active1.target == TargetType.Single){
			currentStage = AbilityStage.GetUnit;
			Logic.Inst.gamePhase = GamePhase.TargetPhase;
			Debug.Log("Entering Target Mode!");
			currentAbility = 1;
			currentRange = active1.range;
		}
	}

	void ActivateAbility2(){
		if(active2.target == TargetType.All || active2.target == TargetType.AoE){
			active2.Activate(gameObject.GetComponent<Unit>());
		}
		else if(active2.target == TargetType.Single){
			currentStage = AbilityStage.GetUnit;
			Logic.Inst.gamePhase = GamePhase.TargetPhase;
			Debug.Log("Entering Target Mode!");
			currentAbility = 2;
			currentRange = active2.range;
		}

		if(type == HeroType.Skadi && hero.Owner.wrathMode){
			skadiWrathCheck = true;
		}
	}

	public void ApplyPassive(){
		if(passive.passive == PassiveType.OneShot){
			passive.Activate(gameObject.GetComponent<Unit>());
		}
	}
}
