using UnityEngine;
using UnityEngine.UI;

public class Logic : MonoBehaviour {

	private static Logic inst;
	private Grid grid;
	private UnitList unitList;
	private InfoPanel infoPanel;

	private Combat combatManager;

	private Unit selectedUnit;
	private Tile selectedTile;

	private Player[] players;
	private int currentPlayer = 0;

	private GamePhase gamePhase = GamePhase.PlacingPhase;	

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

		infoPanel = GetComponent<InfoPanel>();

		if (!infoPanel)
			Debug.LogError("InfoPanel does not exist!");

		infoPanel.Clear();
		infoPanel.UpdateTurnInfo(CurrentPlayer);
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

	public void TileClicked(Tile tile) {
		infoPanel.UpdateTileInfo(tile);

		if (selectedUnit) {
			if(!tile.OccupyngUnit){
				if (selectedUnit.InMoveRange(tile)) {
					grid.GetTile(selectedUnit.Index.x, selectedUnit.Index.y).OccupyngUnit = null;
					selectedUnit.MoveTowardsTile(tile);
					EndTurn();
				}
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

	public void UnitClicked(Unit unit) {
		infoPanel.UpdateUnitInfo(unit);

		if(unit.Owner == CurrentPlayer)
			selectedUnit = unit;
		else if (unit.Owner != CurrentPlayer) { 
			if (selectedUnit) { 
				if (selectedUnit.InAttackRange(unit)) { 
					combatManager.ResolveCombat(selectedUnit, unit);
					if(unit)
						combatManager.ResolveCombat(unit, selectedUnit);

					EndTurn();
				}	
			}
		}
	}

	public void EndTurn() {
		if (currentPlayer + 1 < players.Length)
			currentPlayer++;
		else
			currentPlayer = 0;

		ClearSelected();

		infoPanel.Clear();
		infoPanel.UpdateTurnInfo(CurrentPlayer);
	}

	private void ClearSelected() {
		if(selectedUnit)
			selectedUnit = null;
		if(selectedTile)
			selectedTile = null;
	}

	#region Getters and Setters 	
	public static Logic Inst {
		get { return inst; }
	}

	public Player CurrentPlayer {
		get { return players[currentPlayer]; }
	}

	public Grid Grid {
		get { return grid; }
	}

	public UnitList UnitList {
		get { return unitList; }
	}

	public InfoPanel InfoPanel {
		get { return infoPanel; }
	}

	#endregion
}
