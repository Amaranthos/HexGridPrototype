using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public string playerName = "";
	public int startingFood;
	public int commandPoints;

	public Rect placementField;

	private List<Unit> army = new List<Unit>();

	private int currentFood;
	private int currentCommandPoints;

	public void AddUnit(UnitType type, Tile tile) {
		GameObject temp = (GameObject)Instantiate(Logic.Inst.UnitList.GetUnit(type), tile.transform.position, Quaternion.identity);
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
				if (placementField.CoordsInRange(Logic.Inst.Grid.grid[i, j].Index))
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
		currentCommandPoints = commandPoints;
	}

	#region Getters and Setters
	public string PlayerName {
		get { return playerName; }
	}

	public int CurrentFood {
		get { return currentFood; }
		set { currentFood = value; }
	}

	public int CurrentCommandPoints {
		get { return currentCommandPoints; }
		set { currentCommandPoints = value; }
	}
	#endregion
}
