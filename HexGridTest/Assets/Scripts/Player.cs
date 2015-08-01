using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public string playerName = "";

	public Rect placementField;

	private List<Unit> army = new List<Unit>();

	public void AddUnit(UnitType type, Tile tile) {
		GameObject temp = (GameObject)Instantiate(Logic.Inst.UnitList.GetUnit(type), tile.transform.position, Quaternion.identity);
		Unit unit = temp.GetComponent<Unit>();
		unit.Index = tile.Index;
		unit.Owner = this;
		tile.OccupyngUnit = unit;
		army.Add(unit);
		temp.transform.parent = this.transform;
	}

	public void RemoveUnit(Unit unit) {
		if (army.Contains(unit))
			army.Remove(unit);
	}

	#region Getters and Setters
	public string PlayerName {
		get { return playerName; }
	}
	#endregion
}
