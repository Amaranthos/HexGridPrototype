using UnityEngine;
using System.Collections.Generic;

public class UnitList : MonoBehaviour {

	public List<Unit> units;

	public GameObject GetUnit(UnitType unit) {
		return (units.Find(item => item.type == unit)).gameObject;
	}

	public int LowestCost() {
		int lowest = int.MaxValue;

		for(int i = 0; i < units.Count; i++)
			lowest = Mathf.Min(lowest, units[i].cost);

		return lowest;
	}
}
