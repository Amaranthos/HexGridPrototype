using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Combat : MonoBehaviour {
	public GameObject damageText;
	public Text rollText;
	public float offsetDist, defNumA, defNumB;
	public int spinCount;
	public float turnSpeed;
	private Unit atk,def;
	private bool attaking = false;

	private void Update(){
		if(attaking){
			StartCoroutine("RotateUnits");
		}
	}

	public void ResolveCombat(Unit attacker, Unit defender) {
		atk = attacker;
		def = defender;

		if(atk.type == UnitType.Spearman){
			atk.currentTarget = def;
		}

		// if(def.type == UnitType.Spearman){
		// 	def.currentTarget = atk;
		// }

		attaking = true;

		StartCoroutine("timedCombat");
	}

	public IEnumerator timedCombat(){
		GameObject tempText = null;
		// Logic.Inst.Audio.PlaySFX(SFX.Rune_Roll);
		int hitRoll = 0;

		atk.ChangeAnim(2);
		def.ChangeAnim(4);

		int hitChance = atk.TotalHitChance - def.TotalDodgeChance;
		hitChance = Mathf.FloorToInt((hitChance / 100f * 100f));

		for(int i = 0; i < spinCount; i++){
			yield return new WaitForSeconds(0.05f);
			hitRoll = RollTheDice();
			rollText.text = atk.Owner.hero.type + "'s " + atk.name+ " Rolled... " + hitRoll + "\nNeeds " + (100-hitChance) + "+ To Hit";
		}

		if (hitRoll >= 100-hitChance) {
			int damage = Mathf.RoundToInt(atk.TotalAttack * (1 - ((defNumA * def.TotalDefense)/(defNumB + def.TotalDefense))));
			
			tempText = Instantiate(damageText,(def.gameObject.transform.position + Vector3.up * offsetDist),Quaternion.identity) as GameObject;
			
			if (damage > 0) {
				// Logic.Inst.Audio.PlaySFX(SFX.Attack_Success);
				def.CurrentHitpoints -= damage;
				Debug.Log(atk.Owner.PlayerName + "'s " + atk.type + " does " + damage + " to " + def.Owner.PlayerName + "'s " + def.type);
				//GUIManager.inst.LogCombatResult(atk.Owner.PlayerName + "'s " + atk.type + " does " + damage + " to " + def.Owner.PlayerName + "'s " + def.type);

				tempText.GetComponent<TextMesh>().text = "- " + damage;
				
				if (def.CurrentHitpoints <= 0){
					def.UnitKilled();
//					Logic.Inst.Audio.OnUnitDeath(def,atk);
				}
			}
			else{
				tempText.GetComponent<TextMesh>().text = "- 0";
			}
		}
		else{
			// Logic.Inst.Audio.PlaySFX(SFX.Attack_Fail);
			Debug.Log(atk.Owner.PlayerName + "'s " + atk.type + " misses");
			//GUIManager.inst.LogCombatResult(atk.Owner.PlayerName + "'s " + atk.type + " misses");
			
			tempText = Instantiate(damageText,(def.gameObject.transform.position + Vector3.up * offsetDist),Quaternion.identity) as GameObject;
			tempText.GetComponent<TextMesh>().text = "MISS!";
		}

		yield return new WaitForSeconds(2f);

//		atk.ChangeAnim(0);
//		def.ChangeAnim(0);

		rollText.text = "";
		attaking = false;
	}

	private int RollTheDice(){
		int roll1 = Random.Range(1,25);
		int roll2 = Random.Range(1,25);
		int roll3 = Random.Range(1,25);
		int roll4 = Random.Range(1,25);
		int roll5 = Random.Range(1,25);
		List<int> rolls = new List<int>();
		
		rolls.Add(roll1);
		rolls.Add(roll2);
		rolls.Add(roll3);
		rolls.Add(roll4);
		rolls.Add(roll5);
		
		rolls.Remove(rolls.Min());

		int total = roll1 + roll2 + roll3 + roll4 + roll5 - rolls.Min();

		return total;
	}

	public IEnumerator RotateUnits(){
		Quaternion dir = Quaternion.LookRotation(def.transform.position - atk.transform.position);
		while(atk.transform.rotation != dir){
			atk.transform.rotation = Quaternion.RotateTowards(atk.transform.rotation, dir, Time.deltaTime * 2f * 360f/Mathf.PI * turnSpeed);
			yield return new WaitForEndOfFrame();
        }

		Quaternion dir2 = Quaternion.LookRotation(atk.transform.position - def.transform.position);
		while(def.transform.rotation != dir2){
			def.transform.rotation = Quaternion.RotateTowards(def.transform.rotation, dir2, Time.deltaTime * 2f * 360f/Mathf.PI * turnSpeed);
			yield return new WaitForEndOfFrame();
        }
    }
}
