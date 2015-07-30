using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public string playerName = "";

	private List<Unit> army = new List<Unit>();

	public void AddUnit(UnitType type, Tile tile) {

		GameObject temp = (GameObject)Instantiate(Logic.inst.unitList.GetUnit(type), tile.transform.position, Quaternion.identity);
		tile.IsOccupied = true;
		tile.OccupyngUnit = temp.GetComponent<Unit>();
		army.Add(temp.GetComponent<Unit>());
		temp.transform.parent = this.transform;
		temp.GetComponent<Unit>().Owner = this;
	}

	public string PlayerName {
		get { return playerName; }
	}
}
