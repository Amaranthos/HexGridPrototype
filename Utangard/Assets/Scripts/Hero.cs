using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

	public HeroType type;
	public Ability passive;
	public Ability active1;
	public Ability active2;
	public int currentAbility;
	private Unit target;

	void Start () {
		//gameObject.GetComponent<Unit>().Owner = Logic.Inst.Players[0];	//This is purely for testing. Be sure to remove later.
		//Logic.Inst.Players[0].army.Add(gameObject.GetComponent<Unit>());	//Also for testing.
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			ActivateAbility1();
		}

		if(Input.GetKeyDown(KeyCode.Alpha2)){
			ActivateAbility2();
		}
	}

	public void ReceiveTarget(Unit unit){
		print("Target Received!");
		if(currentAbility == 1){
			active1.Activate(unit);
		}
		else{
			active2.Activate(unit);
		}

		Logic.Inst.gamePhase = GamePhase.CombatPhase;
		print("Back to combat!");
	}

	void ActivateAbility1(){
		if(active1.target == TargetType.All){
			active1.Activate(gameObject.GetComponent<Unit>());
		}
		else if(active1.target == TargetType.Single){
			Logic.Inst.gamePhase = GamePhase.TargetPhase;
			print("Entering Target Mode!");
			currentAbility = 1;
		}
	}

	void ActivateAbility2(){
		if(active2.target == TargetType.All){
			active2.Activate(gameObject.GetComponent<Unit>());
		}
		else if(active2.target == TargetType.Single){
			Logic.Inst.gamePhase = GamePhase.TargetPhase;
			print("Entering Target Mode!");
			currentAbility = 2;
		}
	}
}
