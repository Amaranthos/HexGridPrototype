using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Logic : MonoBehaviour {

	private static Logic inst;
	private Grid grid;
	private UnitList unitList;
	private InfoPanel infoPanel;
	private Audio audio;

	private Combat combatManager;

	private Unit selectedUnit;
	private Tile selectedTile;

	private Player[] players;
	private int currentPlayer = 0;
	private int startingPlayer = 0;

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

		audio = GetComponent<Audio>();

		if (!audio)
			Debug.LogError("Audio manager does not exist!");

		foreach (Player player in players)
			player.StartPlacing();

		infoPanel = GetComponent<InfoPanel>();

		if (!infoPanel)
			Debug.LogError("InfoPanel does not exist!");

		infoPanel.Clear();
		infoPanel.UpdateTurnInfo(CurrentPlayer);
	}

	private void Update() {
		if (Input.GetMouseButtonUp(0)) {
			RaycastHit hit = MouseClick();
			if (hit.transform) {
				GameObject go = hit.transform.gameObject;

				Unit unit = go.GetComponent<Unit>();

				if (unit)
					UnitLClicked(unit);
				else {
					Tile tile = go.GetComponent<Tile>();

					if (tile)
						TileLClicked(tile);
				}

			}
		}

		if (Input.GetMouseButtonUp(1)) {
			RaycastHit hit = MouseClick();
			if (hit.transform) {
				GameObject go = hit.transform.gameObject;

				Unit unit = go.GetComponent<Unit>();

				if (unit)
					UnitRClicked(unit);
				else {
					Tile tile = go.GetComponent<Tile>();

					if (tile)
						TileRClicked(tile);
				}

			}
		}
	}

	private void UnitLClicked(Unit unit) {
		switch (gamePhase) {
			case GamePhase.PlacingPhase:

				break;

			case GamePhase.CombatPhase:

				break;
		}
	}

	private void TileLClicked(Tile tile) {
		switch (gamePhase) {
			case GamePhase.PlacingPhase:

				break;

			case GamePhase.CombatPhase:

				break;
		}
	}

	private void UnitRClicked(Unit unit) {
		switch (gamePhase) {
			case GamePhase.PlacingPhase:

				break;

			case GamePhase.CombatPhase:

				break;
		}
	}

	private void TileRClicked(Tile tile) {
		switch (gamePhase) {
			case GamePhase.PlacingPhase:

				break;

			case GamePhase.CombatPhase:

				break;
		}
	}

	public void SetupGameWorld(int[][] armies) {
		grid.GenerateGrid();		

		for (int i = 0; i < armies.Length; i++){
			List<Tile> tiles = players[i].PlacementField();
			for (int j = 0; j < armies[i].Length; j++)
				players[i].AddUnit((UnitType)armies[i][j], tiles[j], i);
		}

		StartSetupPhase();
	}

	public void StartSetupPhase() {
		currentPlayer = startingPlayer = Random.Range(0, players.Length);

		infoPanel.Enabled(true);
	}

	public void StartCombatPhase() {
		currentPlayer = startingPlayer;
	}

	public void TileClicked(Tile tile) {
		infoPanel.UpdateTileInfo(tile);

		switch (gamePhase) {
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

	public void EndTurn() {
		ChangePlayer();

		switch (gamePhase) {
			case GamePhase.CombatPhase:
				CurrentPlayer.StartTurn();
				break;

			default:
				break;
		}
		
		ClearSelected();				
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
	
	private void ClearSelected() {
		if(selectedUnit)
			selectedUnit = null;
		if(selectedTile)
			selectedTile = null;

		infoPanel.Clear();
	}

	private RaycastHit MouseClick() {
		RaycastHit hit; 
		Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);

		return hit;
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

	public Player[] Players {
		get { return players; }
	}
	#endregion
}