using UnityEngine;
using System.Collections;

public class Logic : MonoBehaviour {

	public static Logic inst;
	public UnitList unitList;

	public Unit selectedUnit;

	private Grid grid;

	public Player player;

	private void Awake() {
		if (!inst)
			inst = this;

		grid = GetComponentInChildren<Grid>();

		if (!grid)
			Debug.LogError("Grid does not exist!");

		unitList = GetComponent<UnitList>();
		if(!unitList)
			Debug.LogError("UnitList does not exist!");
	}

	private void Update() {
		if (Input.GetKeyUp(KeyCode.Space)) {
			player.AddUnit(UnitType.Spearman, grid.GetTile(0,0));
		}
	}

	public void TileSelected(Tile tile) {
		grid.TileSelected();

		if (selectedUnit) {
			selectedUnit.MoveTowardsTile(tile);
		}
	}

	public void UnitSelected(Unit unit) {
		selectedUnit = unit;
	}
}
