using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class Logic : MonoBehaviour {

	private static Logic inst;
	private Grid grid;
	private MapGen mapGen;
	private UnitList unitList;
	private HeroList heroList;
	private TerrainList terrainList;
	//private InfoPanel infoPanel;
	private Audio _audio;
	private Path path;

	private Combat combatManager;

	private Unit selectedUnit;
	private Tile selectedTile;

	private Player[] players;
	public int currentPlayer = -1;
	private int startingPlayer = -1;
	public int winningPlayer = -1;
	private int turnsRemaining;

	private List<Tile> highlightedTiles = new List<Tile>();

	private List<Altar> altars = new List<Altar>();

	public Button endTurn;
	public Button sacrifice;
	
	public GamePhase gamePhase = GamePhase.ArmySelectPhase;

	public int numAltars;
	public int faithPtsPerAltar;
	public int faithPtsPerSacrifice;

	public int turnsForVictory;

	public List<Material> playerColours = new List<Material>();

	//For Damage/Heal Popups
	public GameObject damageText;
	public GameObject healText;
	public GameObject buffText;
	public float offsetDist;

	//For Victory
	public GameObject winText;
	public GameObject timerText;

	private void Awake() {
		if (!inst)
			inst = this;

		grid = GetComponentInChildren<Grid>();
		if (!grid)
			Debug.LogError("Grid does not exist!");

		mapGen = GetComponentInChildren<MapGen>();
		if(!mapGen)
			Debug.LogError("Map Generator does not exist!");

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

		path = GetComponent<Path>();

		if (!path)
			Debug.LogError("Pathfinder does not exist!");

		Camera.main.GetComponent<Vision>().enabled = false;
	}

	private void Update() {
		//Fpr testing random gen
		if(Input.GetKeyUp(KeyCode.Space)){
			mapGen.GenerateMap(grid.TilesList);
		}

		//This currently enables the generic sacrifice button, the button should be included with the player's gui
		if(gamePhase == GamePhase.CombatPhase){
			if(selectedUnit && selectedUnit.CanMove){
				Altar altar = GetAltar(selectedUnit.Index);
				if(altar){
					sacrifice.interactable = true;
				}
			}
			else {
				sacrifice.interactable = false;
			}
		}

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
					else {
						Altar altar = go.GetComponent<Altar>();

						if (altar)
							TileLClicked(grid.TileAt(altar.Index));
					}
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
					else {
						Altar altar = go.GetComponent<Altar>();

						if (altar)
							TileRClicked(grid.TileAt(altar.Index));
					}
				}

			}

			if(gamePhase == GamePhase.TargetPhase){	//So players can back out of an ability cast;
				gamePhase = GamePhase.CombatPhase;
				print ("TARGETING ABORTED");
				ClearHighlightedTiles();
				UnitLClicked(heroList.heroes[currentPlayer].hero);
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
				if(unit.CanMove && unit.Owner == CurrentPlayer && unit.Owner.CommandPoints > 0){
					HighlightMoveRange(unit);
					_audio.PlaySFX(SFX.Unit_Click);
				}
				break;

			case GamePhase.TargetPhase:		//This is horribly ineffecitent. Will likely have to store a record of each hero in logic once selected.
				Hero hero = null;
				foreach (Unit unt in players[currentPlayer].army){
					if(unt.type == UnitType.Hero){
						hero = unt.GetComponent<Hero>();
					}
				}

				hero.CheckTarget(grid.TileAt(unit.Index));	

				if(hero.targets.Count == hero.currentAbility.targets.Count){
					hero.CastAbility();
					gamePhase = GamePhase.CombatPhase;
				}

				print("TARGETING COMPLETE!");
				break;
		}
	}

	private void TileLClicked(Tile tile) {
		if(tile.OccupyingUnit)
			UnitSelected(tile.OccupyingUnit);

		switch (gamePhase) {
			case GamePhase.PlacingPhase:
			_audio.PlaySFX(SFX.Scroll);
				break;

			case GamePhase.CombatPhase:
			_audio.PlaySFX(SFX.Scroll);
				break;

			case GamePhase.TargetPhase:
				Hero hero = null;
				foreach (Unit unit in players[currentPlayer].army){
					if(unit.type == UnitType.Hero){
						hero = unit.GetComponent<Hero>();
					}
				}
				
				hero.CheckTarget(tile);	

				if(hero.targets.Count == hero.currentAbility.targets.Count){
					hero.CastAbility();
					gamePhase = GamePhase.CombatPhase;
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
						if (grid.TileAt(unit.Index).IsPassable){
							SwapUnits(grid.TileAt(unit.Index));
							_audio.PlaySFX(SFX.Unit_Move);
						}

				break;

			case GamePhase.CombatPhase:
				if (selectedUnit && selectedUnit.Owner == CurrentPlayer && selectedUnit.CanMove && selectedUnit.Owner.CommandPoints > 0)
					if (unit.Owner != CurrentPlayer)
						if (selectedUnit.InAttackRange(unit)){
							StartCoroutine(UnitCombat(selectedUnit, unit));
							_audio.PlaySFX(SFX.Rune_Roll);
						}
				break;
		}
	}

	private void TileRClicked(Tile tile) {
		switch (gamePhase) {
			case GamePhase.PlacingPhase:
				if(CurrentPlayer.placementBoundaries.CoordsInRange(tile.Index)){
					if (selectedUnit && selectedUnit.Owner == CurrentPlayer && selectedUnit.CanMove)
						if (tile.IsPassable)
							if (!tile.OccupyingUnit)
								selectedUnit.MoveTowardsTile(tile);
							else if (tile.OccupyingUnit.Owner == CurrentPlayer)
								SwapUnits(tile);
				}
				else
				_audio.PlaySFX(SFX.Unit_CantMoveThere);
				break;

			case GamePhase.CombatPhase:
				if (selectedUnit && selectedUnit.Owner == CurrentPlayer && selectedUnit.CanMove && selectedUnit.Owner.CommandPoints > 0){
					if (selectedUnit.InMoveRange(tile)) {
						if (!tile.OccupyingUnit)
							if(selectedUnit.CurrentMovePoints > 0){
								selectedUnit.MoveTowardsTile(tile);
								ClearSelected();
							}
						else if (tile.OccupyingUnit.Owner != CurrentPlayer)
							StartCoroutine(UnitCombat(selectedUnit, tile.OccupyingUnit));
					}
					else
						_audio.PlaySFX(SFX.Unit_CantMoveThere);
				}
				break;
		}
	}

	private void SwapUnits(Tile tile) {
		_audio.PlaySFX(SFX.Unit_Move);
		Tile prevTile = grid.TileAt(selectedUnit.Index);
		Unit swap = tile.OccupyingUnit;
		swap.MoveTowardsTile(prevTile);
		selectedUnit.MoveTowardsTile(tile);
		prevTile.OccupyingUnit = swap;
	}

	public void SetupGameWorld(int[][] armies) {
		GUIManager.inst.AssignTextures();
		grid.GenerateGrid();

		players[0].placementBoundaries.x = grid.LeastX;
		players[1].placementBoundaries.x = grid.GreatestX - players[1].placementBoundaries.w + 1;

		for(int i = 0; i < players.Length; i++){
			players[i].playerColour = players[i].hero.gameObject.transform.FindChild("window washer/tunic").GetComponent<SkinnedMeshRenderer>().sharedMaterials[0].color;
		}
		
		mapGen.GenerateMap(grid.TilesList);

		for (int i = 0; i < armies.Length; i++){
			List<Tile> tiles = players[i].PlacementField();

			for (int j = 0; j < armies[i].Length; j++){
				players[i].SpawnUnit((UnitType)armies[i][j], tiles[j], i);
			}

			players[i].SpawnHero(tiles[armies[i].Length], i);
		}

		for(int i = 0; i < players.Length; i++){
			if(players[i].hero.passive.passive == PassiveType.Buff){
				players[i].hero.passive.ApplyBuffAll(i);
			}
		}

		SwitchGamePhase(GamePhase.PlacingPhase);
	}

	public void StartSetupPhase() {
		GUIManager.inst.GUICanvas.SetActive(true);

		currentPlayer = startingPlayer = Random.Range(0, players.Length);
		GUIManager.inst.UpdatePlayerGUI(currentPlayer);

		ChangeTileOutlines(CurrentPlayer.PlacementField(), CurrentPlayer.playerColour, 0.1f);

		Camera.main.GetComponent<Vision>().enabled = true;

	}

	public void StartCombatPhase() {
		currentPlayer = startingPlayer;
		GUIManager.inst.UpdatePlayerGUI(currentPlayer);
	}

	private void ChangeTileOutlines(List<Tile> tiles, Color colour, float thickness) {
		for (int i = 0; i < tiles.Count; i++) {
			if (tiles[i]) {
				tiles[i].LineColour(colour);
				tiles[i].LineWidth(thickness);
			}
		}
	}

	private void ChangeTileOutlines(Tile tile, Color colour, float thickness){
		tile.LineColour(colour);
		tile.LineWidth(thickness);
	}

	public void ClearHighlightedTiles() {
		if(highlightedTiles.Count > 0){
			ChangeTileOutlines(highlightedTiles, Color.black, 0.03f);
		}
	}

	public void HighlightMoveRange(Unit unit) {
		ClearHighlightedTiles();
		unit.ClearHighlightedTiles();
		unit.UnitSelected();
	}

	public void HighlightAbilityRange (Skill ability, CubeIndex index){
		ClearHighlightedTiles();

		highlightedTiles = grid.TilesInRange(index, ability.castRange);
		ChangeTileOutlines(highlightedTiles, Color.yellow, 0.1f);
	}

	public void activateHeroAbility(int abilNum){
		if(abilNum == 1){
			CurrentPlayer.hero.ActivateAbility1();
		}
		else{
			CurrentPlayer.hero.ActivateAbility2();
		}
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
		GUIManager.inst.UpdatePlayerGUI(currentPlayer);

		switch (gamePhase) {
			case GamePhase.PlacingPhase:
				prevPlayer.HasFinishedPlacing = true;
				ChangeTileOutlines(prevPlayer.PlacementField(), Color.black, 0.03f);

				if (PlayesPositionedUnits()) {
					SwitchGamePhase(GamePhase.CombatPhase);
					return;
				}

				ChangeTileOutlines(CurrentPlayer.PlacementField(), CurrentPlayer.playerColour, 0.1f);
				break;

			case GamePhase.CombatPhase:
				CheckIfPlayerWinning();

				CurrentPlayer.StartTurn();
				AddFaithPerAltar();

				CurrentPlayer.hero.CalcBuffStrength();
				CurrentPlayer.hero.ApplyPassive();
				
				foreach(Tile tile in grid.TilesList){
					if(CurrentPlayer.hero.type == HeroType.Skadi && tile.Biome == BiomeType.Snow){
						tile.MoveCost = 1;
					}
					else if(tile.Biome == BiomeType.Snow){
						tile.MoveCost = 2;
					}
				}
				break;
		}
	}

	private void CheckIfPlayerWinning() {
		int winning = -1;
		for (int i = 0; i < players.Length; i++){
			if (players[i].capturedAltars.Count == numAltars){
				Debug.Log(players[i].name + " owns all of the altars!");
				winning = i;
			}
		}

		if (winning != -1) {
			if (winning == winningPlayer) {
				turnsRemaining -= 1;

				timerText.SetActive(true);
				timerText.GetComponent<Text>().text = turnsRemaining + " Turns Until " + players[winningPlayer].name + "'s Victory";

				if (turnsRemaining <= 0)
					EndGame();
			}
			else {
				Debug.Log(players[winning].name + " has wrestled control of the altars!");
				winningPlayer = winning;
				turnsRemaining = turnsForVictory;
				timerText.SetActive(false);
				SetWrathMode();
			}
		}
		else {
			SetWrathMode();
			for (int i = 0; i < players.Length; i++){
				if (players[i].army.Count <= 0){
					Debug.Log(players[i].name + " has been eliminated!");
					PlayerEliminated(players[i]);
				}
			}
		}
	}

	private void SetWrathMode() {
		if(winningPlayer != -1){
			players[winningPlayer].wrathMode = false;
			for(int i = 0; i < players.Length; i++)
				if(i != winningPlayer)
					players[i].wrathMode = true;
		}
	}

	public void PlayerEliminated(Player player) {
		player.Defeated = true;

		int countAlive = 0;

		for (int i = 0; i < players.Length; i++)
			if (!players[i].Defeated){
				countAlive++;
				winningPlayer = i;
			}

		if (countAlive <= 1){
			EndGame();
		}
	}

	private void EndGame() {
		gamePhase = GamePhase.FinishedPhase;
		winText.SetActive(true);
		winText.GetComponent<Text>().text = players[winningPlayer].PlayerName + " Wins!";
	}

	private void SwitchGamePhase(GamePhase phase) {
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

	private IEnumerator UnitCombat(Unit att, Unit def) {
		ClearSelected();
		att.CanMove = false;
		combatManager.ResolveCombat(att, def);
		yield return new WaitForSeconds(3.5f);
		if (!def.dead && def.InAttackRange(att))
			combatManager.ResolveCombat(def, att);
	}

	private void ChangePlayer() {
		if (currentPlayer + 1 < players.Length)
			currentPlayer++;
		else
			currentPlayer = 0;

		if (CurrentPlayer.Defeated)
			ChangePlayer();
	}
	
	private void ClearSelected() {

		if(selectedUnit){
			selectedUnit.ClearHighlightedTiles();
			selectedUnit = null;
		}

		ClearHighlightedTiles();
	}

	public void UnitSelected(Unit unit) {
		ClearSelected();

		if(gamePhase == GamePhase.CombatPhase){
			ClearHighlightedTiles();
		}
		selectedUnit = unit;
	}

	private RaycastHit MouseClick() {
		RaycastHit hit; 
		Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);

		return hit;
	}

	private void AddFaithPerAltar() {
		for(int j = 0; j < CurrentPlayer.capturedAltars.Count; j++){
			CurrentPlayer.Faith += faithPtsPerAltar;
		}
	}

	public void SacrificeUnit() {
		selectedUnit.UnitSacrificed();
	}

	public void CaptureAlar(){
		selectedUnit.CaptureAltar();
	}

	IEnumerator ShowAltarText(){
		Text tempText = winText.GetComponent<Text>();
		tempText.text = CurrentPlayer.PlayerName + " HAS CAPTURED AN ALTAR";
		yield return new  WaitForSeconds(3f);
		tempText.text = "";
	}

	#region Getters and Setters 	
	public Altar GetAltar(CubeIndex Index) {
		return altars.Find(item=>item.Index==Index);
	}

	public List<Altar> Altars {
		get {return altars;}
		set {altars = value;}
	}

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

	public Player[] Players {
		get { return players; }
	}

	public Path Path {
		get { return path; }
	}

	public Unit SelectedUnit{
		get { return selectedUnit; }
	}

	public TerrainList Terrains {
		get {return terrainList;}
	}

	public int FaithPerTurn {
		get{ return faithPtsPerAltar * CurrentPlayer.capturedAltars.Count;}
	}
	#endregion
}