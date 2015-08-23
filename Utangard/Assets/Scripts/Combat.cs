using UnityEngine;

public class Combat : MonoBehaviour {

	public void ResolveCombat(Unit attacker, Unit defender) {
		Logic.Inst.Audio.PlaySFX(SFX.Rune_Roll);
		if (Random.Range(0, 100) >= (attacker.TotalHitChance - defender.TotalDodgeChance)) {
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
		else
			Logic.Inst.Audio.PlaySFX(SFX.Attack_Fail);
			Debug.Log(attacker.Owner.PlayerName + "'s " + attacker.type + " misses");
			GUIManager.inst.LogCombatResult(attacker.Owner.PlayerName + "'s " + attacker.type + " misses");
	}
}
