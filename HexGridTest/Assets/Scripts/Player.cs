using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	private string playerName = "";

	private List<Unit> army = new List<Unit>();

	public void AddUnit(UnitType type, Tile tile) {

		GameObject temp = (GameObject)Instantiate(Logic.inst.unitList.GetUnit(type), tile.transform.position, Quaternion.identity);
		army.Add(temp.GetComponent<Unit>());
	}
}
