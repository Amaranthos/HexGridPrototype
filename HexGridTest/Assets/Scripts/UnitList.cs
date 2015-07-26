using UnityEngine;
using System.Collections.Generic;

public class UnitList : MonoBehaviour {

	public List<Unit> units;

	public GameObject GetUnit(UnitType unit) {
		return (units.Find(item => item.type == unit)).gameObject;
	}
}
