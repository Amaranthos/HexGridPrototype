using UnityEngine;

public class Logic : MonoBehaviour {

	public static Logic inst;
	public UnitList unitList;
	public Combat combatManager;

	public Unit selectedUnit;

	private Grid grid;

	private Player[] players;
	private int currentPlayer = 0;

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
		if(players.Length == 0)
			Debug.LogError("No players present!");

		combatManager = GetComponent<Combat>();

		if (!combatManager)
			Debug.LogError("Combat Manager does not exist!");
	}

	private void Update() {
		if (Input.GetKeyUp(KeyCode.Alpha1)) {
			CurrentPlayer.AddUnit(UnitType.Spearman, grid.GetTile(Random.Range(0, grid.gridSize.x), Random.Range(0, grid.gridSize.y)));
		}
		if (Input.GetKeyUp(KeyCode.Alpha2)) {
			CurrentPlayer.AddUnit(UnitType.Axemen, grid.GetTile(Random.Range(0, grid.gridSize.x), Random.Range(0, grid.gridSize.y)));
		}
		if (Input.GetKeyUp(KeyCode.Alpha3)) {
			CurrentPlayer.AddUnit(UnitType.Swordsmen, grid.GetTile(Random.Range(0, grid.gridSize.x), Random.Range(0, grid.gridSize.y)));
		}
	}

	public void TileSelected(Tile tile) {
		grid.TileSelected(tile);

		if (selectedUnit) {
			if(!tile.IsOccupied){
				grid.GetTile(selectedUnit.Index.x, selectedUnit.Index.y).IsOccupied = false;
				grid.GetTile(selectedUnit.Index.x, selectedUnit.Index.y).OccupyngUnit = null;
				selectedUnit.MoveTowardsTile(tile);
				tile.IsSelected = false;
				selectedUnit.IsSelected = false;
				selectedUnit = null;

				EndTurn();
			}
			else {
				if (tile.OccupyngUnit.Owner != selectedUnit.Owner) {
					combatManager.ResolveCombat(selectedUnit, tile.OccupyngUnit);
					if(tile.occupyingUnit)
						combatManager.ResolveCombat(tile.OccupyngUnit, selectedUnit);
					EndTurn();
				}
			}
		}
	}

	public void UnitSelected(Unit unit) {
		selectedUnit = unit;
	}

	public void EndTurn() {
		if (currentPlayer + 1 < players.Length)
			currentPlayer++;
		else
			currentPlayer = 0;
	}


	#region Getters and Setters 	
	public Player CurrentPlayer {
		get { return players[currentPlayer]; }
	}
	#endregion
}
