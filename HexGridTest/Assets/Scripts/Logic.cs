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

	private UnitType unitToBuild = UnitType.None;

	public GamePhase gamePhase = GamePhase.PlacingPhase;

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

		foreach (Player player in players)
			player.StartPlacing();

		infoPanel = GetComponent<InfoPanel>();

		if (!infoPanel)
			Debug.LogError("InfoPanel does not exist!");

		infoPanel.Clear();
		infoPanel.UpdateTurnInfo(CurrentPlayer);
	}

	private void Update() {
		if (Input.GetMouseButtonUp(1))
			ClearSelected();
	}

	public void TileClicked(Tile tile) {
		infoPanel.UpdateTileInfo(tile);

		switch (gamePhase) {
			case GamePhase.PlacingPhase:
				if (CurrentPlayer.placementField.CoordsInRange(tile.Index)){
					if (!tile.OccupyngUnit) { 
						if (unitToBuild != UnitType.None) {
							CurrentPlayer.AddUnit(unitToBuild, tile);
							EndTurn();
						}
					}
				}
				break;

			case GamePhase.CombatPhase:
				if (selectedUnit) {
					if (CurrentPlayer.CurrentCommandPoints > 0) {
						if(!tile.OccupyngUnit) {
							if (selectedUnit.InMoveRange(tile)) {
								grid.GetTile(selectedUnit.Index.x, selectedUnit.Index.y).OccupyngUnit = null;
								selectedUnit.MoveTowardsTile(tile);
							}
						}
						else {
							if (tile.OccupyngUnit.Owner != selectedUnit.Owner) {
								UnitCombat(selectedUnit, tile.OccupyngUnit);
							}
						}
					}
				}
				break;

			default:
				break;
		}
		infoPanel.UpdateTurnInfo(CurrentPlayer);
	}

	public void UnitClicked(Unit unit) {
		switch (gamePhase) {
			case GamePhase.PlacingPhase:
				break;

			case GamePhase.CombatPhase:
				infoPanel.UpdateUnitInfo(unit);

				if(unit.Owner == CurrentPlayer)
					selectedUnit = unit;
				else if (unit.Owner != CurrentPlayer) {
					if (CurrentPlayer.CurrentCommandPoints > 0) {
						if (selectedUnit && selectedUnit.InAttackRange(unit)) { 
							UnitCombat(selectedUnit, unit);	
						}
					}
				}
				break;

			default:
				break;
		}
		infoPanel.UpdateTurnInfo(CurrentPlayer);
	}

	private void UnitCombat(Unit att, Unit def) {
		combatManager.ResolveCombat(att, def);
		if (def)
			combatManager.ResolveCombat(def, att);

		att.Owner.CurrentCommandPoints--;

		if (att.Owner.CurrentCommandPoints == 0)
			EndTurn();
	}

	private void ChangePlayer() {
		if (currentPlayer + 1 < players.Length)
			currentPlayer++;
		else
			currentPlayer = 0;
	}

	private bool PlayersCanPlace() {
		bool success = false;

		foreach(Player player in players)
			if(player.CurrentFood > unitList.LowestCost())
				success = true;

		return success;
	}

	public void EndTurn() {
		ChangePlayer();

		switch (gamePhase) {
			case GamePhase.PlacingPhase:
				if (PlayersCanPlace()) {
					if (CurrentPlayer.CurrentFood < unitList.LowestCost())
						ChangePlayer();
				}
				else {
					CurrentPlayer.StartTurn();
					gamePhase = GamePhase.CombatPhase;
				}
				break;

			case GamePhase.CombatPhase:
				CurrentPlayer.StartTurn();
				break;

			default:
				break;
		}
		
		ClearSelected();
				
		infoPanel.UpdateTurnInfo(CurrentPlayer);
	}

	public void UpdateBuildUnit(GameObject unit) {
		unitToBuild = unit.GetComponent<Unit>().type;
	}

	private void ClearSelected() {
		if(selectedUnit)
			selectedUnit = null;
		if(selectedTile)
			selectedTile = null;

		unitToBuild = UnitType.None;

		infoPanel.Clear();
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