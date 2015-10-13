using UnityEngine;

public class Combat : MonoBehaviour {

	public void ResolveCombat(Unit attacker, Unit defender) {
		Logic.Inst.Audio.PlaySFX(SFX.Rune_Roll);

		int hitRoll = Random.Range(1, 100);
		int hitChance = attacker.TotalHitChance - defender.TotalDodgeChance;
		hitChance = Mathf.FloorToInt((hitChance / 100f * 100f));

		// Debug.Log("Attacker Hit Chance: " + attacker.TotalHitChance + " Defender Dodge Chance: " + defender.TotalDodgeChance + " Total Hit Chance: " + hitChance + " Roll: " + hitRoll);

		if (hitRoll >= 100-hitChance) {
			int damage = attacker.TotalAttack - defender.TotalDefense;
			if (damage > 0) {
				Logic.Inst.Audio.PlaySFX(SFX.Attack_Success);
				defender.CurrentHitpoints -= damage;
				Debug.Log(attacker.Owner.PlayerName + "'s " + attacker.type + " does " + damage + " to " + defender.Owner.PlayerName + "'s " + defender.type);
				GUIManager.inst.LogCombatResult(attacker.Owner.PlayerName + "'s " + attacker.type + " does " + damage + " to " + defender.Owner.PlayerName + "'s " + defender.type);
				
				if (defender.CurrentHitpoints <= 0)
					defender.UnitKilled();
			}
		}
		else{
			Logic.Inst.Audio.PlaySFX(SFX.Attack_Fail);
			Debug.Log(attacker.Owner.PlayerName + "'s " + attacker.type + " misses");
			GUIManager.inst.LogCombatResult(attacker.Owner.PlayerName + "'s " + attacker.type + " misses");
		}
	}
}
