﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitHealthBar : MonoBehaviour {

	private Slider hpBar;
	private Unit unit;
	private int tempHp;
	private bool updatingHP;
	private bool cooldown;
	private float counter;
	private Quaternion initRot;

	void Start () {
		hpBar = GetComponentInChildren<Slider>();
		unit = GetComponentInParent<Unit>();
		hpBar.maxValue = unit.maxHitpoints;
		tempHp = unit.CurrentHitpoints;
		hpBar.value = tempHp;
		UpdateRotation();
		initRot = transform.rotation;
	}

	void Update () {
		UpdateHealthBar();
		if(unit.CurrentHitpoints < 1){
			hpBar.fillRect.gameObject.GetComponent<Image>().enabled = false;
		}
	}

	void LateUpdate () {
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Camera.main.transform.rotation, 99);
		//UpdatePosition();
	}

	void UpdateHealthBar(){
		if(tempHp != unit.CurrentHitpoints){
			updatingHP = true;
		}
		else{
			updatingHP = false;
		}

		if(updatingHP){
			if(counter >= 0.000000000000000000000001){
				counter = 0;
				if(unit.CurrentHitpoints < tempHp){
					tempHp--;
				}
				else{
					tempHp++;
				}
				hpBar.value = tempHp;
			}
			else{
				counter += Time.deltaTime;
			}
		}
	}

	void UpdateRotation(){
		if(unit.Owner == Logic.Inst.Players[0]){
			transform.localPosition = new Vector3(Mathf.Abs(transform.localPosition.x), transform.localPosition.y, transform.localPosition.z);
			transform.rotation = Quaternion.Euler(30,0,0);
		}
	}

	void UpdatePosition(){
		transform.rotation = initRot;
	}
}
