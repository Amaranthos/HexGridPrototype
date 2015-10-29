using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Combat : MonoBehaviour {
	public GameObject damageText;
	public Text rollText;
	public float offsetDist, defNumA, defNumB;
	public int spinCount;
	private Unit atk,def;

	public void ResolveCombat(Unit attacker, Unit defender) {
		atk = attacker;
		def = defender;
		StartCoroutine("timedCombat");

//		GameObject tempText = null;
//		Logic.Inst.Audio.PlaySFX(SFX.Rune_Roll);

//		int hitRoll = Random.Range(1, 100);
//		int hitChance = attacker.TotalHitChance - defender.TotalDodgeChance;
//		hitChance = Mathf.FloorToInt((hitChance / 100f * 100f));

		// Debug.Log("Attacker Hit Chance: " + attacker.TotalHitChance + " Defender Dodge Chance: " + defender.TotalDodgeChance + " Total Hit Chance: " + hitChance + " Roll: " + hitRoll);

//		if (hitRoll >= 100-hitChance) {
//			int damage = attacker.TotalAttack - defender.TotalDefense;
//
//			tempText = Instantiate(damageText,(defender.gameObject.transform.position + Vector3.up * offsetDist),Quaternion.identity) as GameObject;
//			tempText.GetComponent<TextMesh>().text = "- " + damage;
//
//			if (damage > 0) {
//				Logic.Inst.Audio.PlaySFX(SFX.Attack_Success);
//				defender.CurrentHitpoints -= damage;
//				Debug.Log(attacker.Owner.PlayerName + "'s " + attacker.type + " does " + damage + " to " + defender.Owner.PlayerName + "'s " + defender.type);
//				GUIManager.inst.LogCombatResult(attacker.Owner.PlayerName + "'s " + attacker.type + " does " + damage + " to " + defender.Owner.PlayerName + "'s " + defender.type);
//				
//				if (defender.CurrentHitpoints <= 0)
//					defender.UnitKilled();
//			}
//		}
//		else{
//			Logic.Inst.Audio.PlaySFX(SFX.Attack_Fail);
//			Debug.Log(attacker.Owner.PlayerName + "'s " + attacker.type + " misses");
//			GUIManager.inst.LogCombatResult(attacker.Owner.PlayerName + "'s " + attacker.type + " misses");
//
//			tempText = Instantiate(damageText,(defender.gameObject.transform.position + Vector3.up * offsetDist),Quaternion.identity) as GameObject;
//			tempText.GetComponent<TextMesh>().text = "MISS!";
//		}
	}

	public IEnumerator timedCombat(){
		GameObject tempText = null;
		Logic.Inst.Audio.PlaySFX(SFX.Rune_Roll);
		int hitRoll = 0;

		atk.ChangeAnim(2);
		def.ChangeAnim(0);	//Will need to be state 4 when defend animation added.

		int hitChance = atk.TotalHitChance - def.TotalDodgeChance;
		hitChance = Mathf.FloorToInt((hitChance / 100f * 100f));

		for(int i = 0; i < spinCount; i++){
			yield return new WaitForSeconds(0.05f);
			hitRoll = Random.Range(1, 100);
			rollText.text = atk.Owner.hero.type + "'s " + atk.type + " Rolled... " + hitRoll + "\nNeeds " + (100-hitChance) + "+ To Hit";
		}

		if (hitRoll >= 100-hitChance) {
			int damage = Mathf.RoundToInt(atk.TotalAttack * (1 - ((defNumA * def.TotalDefense)/(defNumB + def.TotalDefense))));
			
			tempText = Instantiate(damageText,(def.gameObject.transform.position + Vector3.up * offsetDist),Quaternion.identity) as GameObject;
			
			if (damage > 0) {
				Logic.Inst.Audio.PlaySFX(SFX.Attack_Success);
				def.CurrentHitpoints -= damage;
				Debug.Log(atk.Owner.PlayerName + "'s " + atk.type + " does " + damage + " to " + def.Owner.PlayerName + "'s " + def.type);
				GUIManager.inst.LogCombatResult(atk.Owner.PlayerName + "'s " + atk.type + " does " + damage + " to " + def.Owner.PlayerName + "'s " + def.type);

				tempText.GetComponent<TextMesh>().text = "- " + damage;
				
				if (def.CurrentHitpoints <= 0)
					def.UnitKilled();
			}
			else{
				tempText.GetComponent<TextMesh>().text = "- 0";
			}
		}
		else{
			Logic.Inst.Audio.PlaySFX(SFX.Attack_Fail);
			Debug.Log(atk.Owner.PlayerName + "'s " + atk.type + " misses");
			GUIManager.inst.LogCombatResult(atk.Owner.PlayerName + "'s " + atk.type + " misses");
			
			tempText = Instantiate(damageText,(def.gameObject.transform.position + Vector3.up * offsetDist),Quaternion.identity) as GameObject;
			tempText.GetComponent<TextMesh>().text = "MISS!";
		}

		yield return new WaitForSeconds(2f);

//		atk.ChangeAnim(0);
//		def.ChangeAnim(0);

		rollText.text = "";
	}
}
