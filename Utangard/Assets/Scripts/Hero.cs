using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

	public HeroType type;
	public Ability passive;
	public Ability active1;
	public Ability active2;
	private Unit target;

	void Start () {
		gameObject.GetComponent<Unit>().Owner = Logic.Inst.Players[0];	//This is purely for testing. Be sure to remove later.
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			active1.Activate(target);
		}

		if(Input.GetKeyDown(KeyCode.Alpha2)){
			active2.Activate(gameObject.GetComponent<Unit>());
		}
	}
}
