using UnityEngine;
using System.Collections;

public class Logic : MonoBehaviour {

	public static Logic inst;
	public UnitList unitList;

	public Unit selectedUnit;

	private Grid grid;

	public Player[] players;
	public int currentPlayer = 0;

	private void Awake() {
		if (!inst)
			inst = this;

		grid = GetComponentInChildren<Grid>();

		if (!grid)
			Debug.LogError("Grid does not exist!");

		unitList = GetComponent<UnitList>();
		if(!unitList)
			Debug.LogError("UnitList does not exist!");

		players = GetComponentsInChildren<Player>();
	}

	private void Update() {
		if (Input.GetKeyUp(KeyCode.Alpha1)) {
			players[currentPlayer].AddUnit(UnitType.Spearman, grid.GetTile(Random.Range(0, grid.gridSize.x), Random.Range(0, grid.gridSize.y)));
		}
		if (Input.GetKeyUp(KeyCode.Alpha2)) {
			players[currentPlayer].AddUnit(UnitType.Axemen, grid.GetTile(Random.Range(0, grid.gridSize.x), Random.Range(0, grid.gridSize.y)));
		}
		if (Input.GetKeyUp(KeyCode.Alpha3)) {
			players[currentPlayer].AddUnit(UnitType.Swordsmen, grid.GetTile(Random.Range(0, grid.gridSize.x), Random.Range(0, grid.gridSize.y)));
		}
	}

	public void TileSelected(Tile tile) {
		grid.TileSelected(tile);

		if (selectedUnit) {
			if(!tile.isOccupied){
				grid.GetTile(selectedUnit.Index.x, selectedUnit.Index.y).isOccupied = false;
				selectedUnit.MoveTowardsTile(tile);
				tile.IsSelected = false;
				selectedUnit.IsSelected = false;
				selectedUnit = null;

				EndTurn();
			}
		}
	}

	public void UnitSelected(Unit unit) {
		selectedUnit = unit;
	}

	private void EndTurn() {
		if (currentPlayer < players.Length)
			currentPlayer++;
		else
			currentPlayer = 0;
	}

}
