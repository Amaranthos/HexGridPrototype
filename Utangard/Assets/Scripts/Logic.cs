using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Logic : MonoBehaviour {

	private static Logic inst;
	private Grid grid;
	private UnitList unitList;
	private HeroList heroList;
	private TerrainList terrainList;
	private InfoPanel infoPanel;
	private Audio _audio;
	private Path path;

	private Combat combatManager;

	private Unit selectedUnit;
	private Tile selectedTile;

	private Player[] players;
	[SerializeField]
	private int currentPlayer = 0;
	[SerializeField]
	private int startingPlayer = 0;

	private List<Tile> highlightedTiles = new List<Tile>();

	public Button endTurn;
	
	public GamePhase gamePhase = GamePhase.ArmySelectPhase;

	public int numAltars;

	private void Awake() {
		if (!inst)
			inst = this;

		grid = GetComponentInChildren<Grid>();

		if (!grid)
			Debug.LogError("Grid does not exist!");

		unitList = GetComponent<UnitList>();
		if(!unitList)
			Debug.LogError("UnitList does not exist!");

		heroList = GetComponent<HeroList>();
		if (!heroList)
			Debug.LogError("HeroList does not exist!");

		terrainList = GetComponent<TerrainList>();
		if (!terrainList)
			Debug.LogError("TerrainList does not exist!");

		players = GetComponentsInChildren<Player>();
		if(players.Length == 0)
			Debug.LogError("No players present!");

		combatManager = GetComponent<Combat>();

		if (!combatManager)
			Debug.LogError("Combat Manager does not exist!");

		_audio = GetComponent<Audio>();

		if (!_audio)
			Debug.LogError("Audio manager does not exist!");

		foreach (Player player in players)
			player.StartPlacing();

		infoPanel = GetComponent<InfoPanel>();

		if (!infoPanel)
			Debug.LogError("InfoPanel does not exist!");

		path = GetComponent<Path>();

		if (!path)
			Debug.LogError("Pathfinder does not exist!");

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

			if(gamePhase == GamePhase.TargetPhase){	//So players can back out of an ability cast;
				gamePhase = GamePhase.CombatPhase;
				print ("TARGETING ABORTED");
			}
		}
	}

	private void UnitLClicked(Unit unit) {
		UnitSelected(unit);

		switch (gamePhase) {
			case GamePhase.PlacingPhase:
			_audio.PlaySFX(SFX.Unit_Click);
				break;

			case GamePhase.CombatPhase:
				if(!unit.HasAttacked && unit.Owner == CurrentPlayer){
					HighlightMoveRange(unit);
					_audio.PlaySFX(SFX.Unit_Click);
				}
				break;

			case GamePhase.TargetPhase:		//This is horribly ineffecitent. Will likely have to store a record of each hero in logic once selected.
				foreach (Unit unt in players[currentPlayer].army){
					if(unt.type == UnitType.Hero){
						unt.GetComponent<Hero>().ReceiveTarget(unit);
						print("FOUND TARGET!");
					}
				}
				print("TARGETING COMPLETE!");
				break;
		}
	}

	private void TileLClicked(Tile tile) {
		TileSelected(tile);

		switch (gamePhase) {
			case GamePhase.PlacingPhase:
			_audio.PlaySFX(SFX.Scroll);
				break;

			case GamePhase.CombatPhase:
			_audio.PlaySFX(SFX.Scroll);
				break;

			case GamePhase.TargetPhase:
				foreach (Unit unit in players[currentPlayer].army){
					if(unit.type == UnitType.Hero){
//						unit.GetComponent<Hero>().ReceiveTarget(//THE UNIT ON THE TILE.);
						print("FOUND TARGET!");
					}
				}
				print("TARGETING COMPLETE!");
				break;
		}
	}

	private void UnitRClicked(Unit unit) {
		switch (gamePhase) {
			case GamePhase.PlacingPhase:
				if (CurrentPlayer.placementBoundaries.CoordsInRange(unit.Index))
					if (selectedUnit && selectedUnit.Owner == CurrentPlayer)
						if (grid.GetTile(unit.Index).IsPassable){
							SwapUnits(grid.GetTile(unit.Index));
							_audio.PlaySFX(SFX.Unit_Move);
							}

				break;

			case GamePhase.CombatPhase:
				if (selectedUnit && selectedUnit.Owner == CurrentPlayer && !selectedUnit.HasAttacked)
					if (unit.Owner != CurrentPlayer)
						if (selectedUnit.InAttackRange(unit)){
							UnitCombat(selectedUnit, unit);
							_audio.PlaySFX(SFX.Rune_Roll);
							}
				break;
		}
	}

	private void TileRClicked(Tile tile) {
		switch (gamePhase) {
			case GamePhase.PlacingPhase:
				if(CurrentPlayer.placementBoundaries.CoordsInRange(tile.Index)){
					if (selectedUnit && selectedUnit.Owner == CurrentPlayer && !selectedUnit.HasAttacked)
						if (tile.IsPassable)
							if (!tile.OccupyngUnit)
								selectedUnit.MoveTowardsTile(tile);
							else if (tile.OccupyngUnit.Owner == CurrentPlayer)
								SwapUnits(tile);
				}
				else
				_audio.PlaySFX(SFX.Unit_CantMoveThere);
				break;

			case GamePhase.CombatPhase:
				if (selectedUnit && selectedUnit.Owner == CurrentPlayer && !selectedUnit.HasAttacked){
					if (selectedUnit.InMoveRange(tile))
					{
						if (!tile.OccupyngUnit) {
							selectedUnit.MoveTowardsTile(tile);
							HighlightMoveRange(selectedUnit);
						}
						else if (tile.OccupyngUnit.Owner != CurrentPlayer)
							UnitCombat(selectedUnit, tile.OccupyngUnit);
					}
					else
						_audio.PlaySFX(SFX.Unit_CantMoveThere);
				}
				break;
		}
	}

	private void SwapUnits(Tile tile) {
		_audio.PlaySFX(SFX.Unit_Move);
		Tile prevTile = grid.GetTile(selectedUnit.Index);
		Unit swap = tile.OccupyngUnit;
		swap.MoveTowardsTile(prevTile);
		selectedUnit.MoveTowardsTile(tile);
		prevTile.OccupyngUnit = swap;
	}

	public void SetupGameWorld(int[][] armies) {
		GUIManager.inst.AssignTextures();
		grid.GenerateGrid();

		for (int i = 0; i < armies.Length; i++){
			List<Tile> tiles = players[i].PlacementField();

			for (int j = 0; j < armies[i].Length; j++)
				players[i].SpawnUnit((UnitType)armies[i][j], tiles[j], i);

			players[i].SpawnHero(tiles[armies[i].Length], i);
		}

		for (int i = 0; i < numAltars; i++) {
			Tile rand = Grid.GetTile(Random.Range(0, Grid.gridSize.x), Random.Range(0, Grid.gridSize.y));

			Instantiate(terrainList.GetAltar(), rand.transform.position, Quaternion.Euler(Vector3.up * i * 45));
		}

			SwtichGamePhase(GamePhase.PlacingPhase);
	}

	public void StartSetupPhase() {
		GUIManager.inst.GUICanvas.SetActive(true);

		currentPlayer = startingPlayer = Random.Range(0, players.Length);
		GUIManager.inst.UpdatePlayerGUI(currentPlayer);

		ChangeTileOutlines(CurrentPlayer.PlacementField(), CurrentPlayer.playerColour, 0.06f);

		Camera.main.GetComponent<Vision>().enabled = true;

		infoPanel.Enabled(true);
		infoPanel.UpdateTurnInfo(CurrentPlayer);
	}

	public void StartCombatPhase() {
		currentPlayer = startingPlayer;
		GUIManager.inst.UpdatePlayerGUI(currentPlayer);
		infoPanel.UpdateTurnInfo(CurrentPlayer);
	}

	private void ChangeTileOutlines(List<Tile> tiles, Color colour, float thickness) {
		for (int i = 0; i < tiles.Count; i++) {
			if (tiles[i]) {
				tiles[i].SetLineColour(colour);
				tiles[i].SetWidth(thickness);
			}
		}
	}

	private void ClearHighlightedTiles() {
		ChangeTileOutlines(highlightedTiles, Color.black, 0.03f);
	}

	private void HighlightMoveRange(Unit unit) {
		ClearHighlightedTiles();

		highlightedTiles = grid.TilesInRange(unit.Index, unit.movePoints);
		ChangeTileOutlines(highlightedTiles, Color.green, 0.06f);
	}

	public bool PlayesPositionedUnits() {
		bool placingFinished = true;

		for (int i = 0; i < players.Length; i++)
			if (!players[i].HasFinishedPlacing)
				placingFinished = false;

		return placingFinished;
	}

	public void EndTurn() {
		Player prevPlayer = CurrentPlayer;
		ChangePlayer();

		ClearSelected();
		infoPanel.UpdateTurnInfo(CurrentPlayer);
		GUIManager.inst.UpdatePlayerGUI(currentPlayer);

		switch (gamePhase) {
			case GamePhase.PlacingPhase:
				prevPlayer.HasFinishedPlacing = true;
				ChangeTileOutlines(prevPlayer.PlacementField(), Color.black, 0.03f);

				if (PlayesPositionedUnits()) {
					SwtichGamePhase(GamePhase.CombatPhase);
					return;
				}

				ChangeTileOutlines(CurrentPlayer.PlacementField(), CurrentPlayer.playerColour, 0.06f);
				break;

			case GamePhase.CombatPhase:
				
				CurrentPlayer.StartTurn();
				break;
		}
	}

	private void SwtichGamePhase(GamePhase phase) {
		gamePhase = phase;

		switch (phase) {
			case GamePhase.PlacingPhase:
				StartSetupPhase();
				break;

			case GamePhase.CombatPhase:
				StartCombatPhase();
				break;
		}
	}

	private void UnitCombat(Unit att, Unit def) {
		ClearSelected();
		att.HasAttacked = true;
		combatManager.ResolveCombat(att, def);
		if (def)
			combatManager.ResolveCombat(def, att);
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

		ClearHighlightedTiles();

		infoPanel.Clear();
	}

	private void UnitSelected(Unit unit) {
		if(gamePhase == GamePhase.CombatPhase)
			ClearHighlightedTiles();
		selectedUnit = unit;
		infoPanel.UpdateUnitInfo(unit);
	}

	private void TileSelected(Tile tile) {
		selectedTile = tile;
		infoPanel.UpdateTileInfo(tile);
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

	// I added this because I wanted to know the number of the current player - Callan
	public int CurrentPlayerNum {
		get { return currentPlayer; }
	}

	public Audio Audio{
		get { return _audio; }
	}

	public Grid Grid {
		get { return grid; }
	}

	public UnitList UnitList {
		get { return unitList; }
	}

	public HeroList HeroList {
		get { return heroList; }
	}

	public InfoPanel InfoPanel {
		get { return infoPanel; }
	}

	public Player[] Players {
		get { return players; }
	}

	public Path Path {
		get { return path; }
	}
	#endregion
}