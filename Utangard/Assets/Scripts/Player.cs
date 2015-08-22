using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public string playerName = "";
	public int startingFood;
	public Color playerColour;

	public Rect placementBoundaries;

	public bool wrathMode = false;

	public List<Unit> army = new List<Unit>();
	public Hero hero;

	private int currentFood;

	[SerializeField]
	private bool hasFinishedPlacing = false;

	public void AddUnit(UnitType type, Tile tile, int index) {
		GameObject temp = (GameObject)Instantiate(Logic.Inst.UnitList.GetUnit(type), tile.transform.position, Quaternion.Euler(Vector3.up * index * 180));
		Unit unit = temp.GetComponent<Unit>();
		unit.Index = tile.Index;
		unit.Owner = this;
		tile.OccupyngUnit = unit;
		army.Add(unit);
		temp.transform.parent = this.transform;
	}

	public List<Tile> PlacementField() {
		List<Tile> tiles = new List<Tile>();
		for (int i = 0; i < Logic.Inst.Grid.gridSize.x; i++)
			for (int j = 0; j < Logic.Inst.Grid.gridSize.y; j++)
				if (placementBoundaries.CoordsInRange(Logic.Inst.Grid.grid[i, j].Index))
					tiles.Add(Logic.Inst.Grid.grid[i, j]);

		return tiles;
	}

	public void RemoveUnit(Unit unit) {
		if (army.Contains(unit))
			army.Remove(unit);
	}

	public void StartPlacing() {
		currentFood = startingFood;
	}

	public void StartTurn() {
		for (int i = 0; i < army.Count; i++) {
			army[i].HasAttacked = false;
		}

		foreach(Unit unit in army){		//Just for testing. I tried it in the above loop but it was called on every unit mulitple times.
			unit.OnTurnStart();
			
			Debug.Log(unit.type + " Starting Turn");
		}
	}

	#region Getters and Setters
	public string PlayerName {
		get { return playerName; }
	}

	public int CurrentFood {
		get { return currentFood; }
		set { currentFood = value; }
	}

	public bool HasFinishedPlacing {
		get { return hasFinishedPlacing; }
		set { hasFinishedPlacing = value; }
	}
	#endregion
}
