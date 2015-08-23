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
	private Unit target;

	void Start () {
		//gameObject.GetComponent<Unit>().Owner = Logic.Inst.Players[0];	//This is purely for testing. Be sure to remove later.
		//Logic.Inst.Players[0].army.Add(gameObject.GetComponent<Unit>());	//Also for testing.
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1) && gameObject.GetComponent<Unit>().Owner == Logic.Inst.Players[0]){
			ActivateAbility1();
		}

		if(Input.GetKeyDown(KeyCode.Alpha2) && gameObject.GetComponent<Unit>().Owner == Logic.Inst.Players[0]){
			ActivateAbility2();
		}

		if(Input.GetKeyDown(KeyCode.Alpha3) && gameObject.GetComponent<Unit>().Owner == Logic.Inst.Players[1]){
			ActivateAbility1();
		}

		if(Input.GetKeyDown(KeyCode.Alpha3) && gameObject.GetComponent<Unit>().Owner == Logic.Inst.Players[1]){
			ActivateAbility2();
		}
	}

	public void ReceiveTarget(Unit unit){
		List<Tile> inRange = new List<Tile>();

		print("Target Received!");
		inRange = Logic.Inst.Grid.AbilityRange(gameObject.GetComponent<Unit>().Index,currentRange);

		if(inRange.Contains(Logic.Inst.Grid.GetTile(unit.Index))){
			if(currentAbility == 1){
				active1.Activate(unit);
			}
			else{
				active2.Activate(unit);
			}
		}
		Logic.Inst.gamePhase = GamePhase.CombatPhase;
		print("Back to combat!");
	}

	void ActivateAbility1(){
		if(active1.target == TargetType.All || active1.target == TargetType.AoE){
			active1.Activate(gameObject.GetComponent<Unit>());
		}
		else if(active1.target == TargetType.Single){
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
			Logic.Inst.gamePhase = GamePhase.TargetPhase;
			Debug.Log("Entering Target Mode!");
			currentAbility = 2;
			currentRange = active2.range;
		}
	}

	public void ApplyPassive(){
		if(passive.passive == PassiveType.OneShot){
			passive.Activate(gameObject.GetComponent<Unit>());
		}
	}
}
