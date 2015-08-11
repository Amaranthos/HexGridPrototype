using UnityEngine;

public class Combat : MonoBehaviour {

	public void ResolveCombat(Unit attacker, Unit defender) {
		if (Random.Range(0, 100) >= (attacker.TotalHitChance - defender.TotalDodgeChance)) {
			int damage = attacker.TotalAttack - defender.TotalDefense;
			if (damage > 0) {
				defender.CurrentHitpoints -= damage;
				Debug.Log(attacker.Owner.PlayerName + "'s " + attacker.type + " does " + damage + " to " + defender.Owner.PlayerName + "'s " + defender.type);
				if (defender.CurrentHitpoints <= 0)
					defender.UnitKilled();
			}
		}
		else
			Debug.Log(attacker.Owner.PlayerName + "'s " + attacker.type + " misses");
	}
}
