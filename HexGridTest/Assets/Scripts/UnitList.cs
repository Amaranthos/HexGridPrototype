using UnityEngine;
using System.Collections.Generic;

public class UnitList : MonoBehaviour {

	public List<Unit> units;

	public GameObject GetUnit(UnitType unit) {
		return (units.Find(item => item.type == unit)).gameObject;
	}

	public int LowestCost() {
		int lowest = int.MaxValue;

		foreach (Unit unit in units)
			lowest = Mathf.Min(lowest, unit.cost);

		return lowest;
	}
}
