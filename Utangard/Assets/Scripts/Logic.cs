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
	public HeroList heroList;
	private TerrainList terrainList;
	private Formations formations;
	//private InfoPanel infoPanel;
	private Audio _audio;
	private Path path;
	private MusicPlayer music;

	private Combat combatManager;

	private Unit selectedUnit;
	private Tile selectedTile;

	private Player[] players;
	public int currentPlayer = -1;
	private int startingPlayer = -1;
	public int winningPlayer = -1;
	public int turnsRemaining;

	private List<Tile> highlightedTiles = new List<Tile>();

	private List<Altar> altars = new List<Altar>();

	public Button endTurn;
	public Button sacrifice;
	
	public GamePhase gamePhase = GamePhase.ArmySelectPhase;

	public int numAltars;
	public int faithPtsPerAltar;
	public int faithPtsPerSacrifice;

	public int turnsForVictory;

	//For Damage/Heal Popups
	public GameObject damageText;
	public GameObject healText;
	public GameObject buffText;
	public float offsetDist;

	//For Victory
	public GameObject returnButton;
	public GameObject winText;
	public GameObject timerText;
	public List<GameObject> gameGui = new List<GameObject>();

	public GameObject enviro;

	public int firstPlayerFaith;
	public int secondPlayerFaith;

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

		for(int i = 0; i < players.Length; i++){
			players[i].StartPlacing();
		}

		path = GetComponent<Path>();

		if (!path)
			Debug.LogError("Pathfinder does not exist!");

		formations = GetComponent<Formations>();

		if(!formations)
			Debug.LogError("Formation object does not exist!");

		music = Camera.main.gameObject.GetComponentInChildren<MusicPlayer>();

		if (!music)
			Debug.LogError("Music Player does not exist!");

		Camera.main.GetComponent<Vision>().enabled = false;
	}

	private void Start() {
		music.ChangeBase(MusicBaseState.Title);

		turnsRemaining = turnsForVictory;
	}

	private void Update() {
		//This currently enables the generic sacrifice button, the button should be included with the player's gui
		if(gamePhase == GamePhase.CombatPhase){
			if(selectedUnit && selectedUnit.CanMove){
				Altar altar = GetAltar(selectedUnit.Index);
				if(altar){
					//sacrifice.interactable = true;
				}
			}
			else {
				//sacrifice.interactable = false;
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

		if(Input.GetKeyDown(KeyCode.Return)){
			music.Mute();
		}

		if(Input.GetKeyDown(KeyCode.RightShift)){
			music.Unmute();
		}


		if(gamePhase == GamePhase.PlacingPhase){
			if(Input.GetKeyDown(KeyCode.Alpha1)){
				formations.form = UnitFormations.Aggressive;
				formations.Reform(currentPlayer, CurrentPlayer.units);
			}

			if(Input.GetKeyDown(KeyCode.Alpha2)){
				formations.form = UnitFormations.Defensive;
				formations.Reform(currentPlayer, CurrentPlayer.units);
			}

			if(Input.GetKeyDown(KeyCode.Alpha3)){
				formations.form = UnitFormations.SkirmishAgg;
				formations.Reform(currentPlayer, CurrentPlayer.units);
			}
		}
	}

	public void ReformUnits(int formation){
		formations.form = (UnitFormations) formation;
		formations.Reform(currentPlayer, CurrentPlayer.units);
	}

	public void UnitLClicked(Unit unit) {
		if(selectedUnit && gamePhase == GamePhase.CombatPhase){
			selectedUnit.OnDeselect();
		}
		UnitSelected(unit);

		switch (gamePhase) {
			case GamePhase.PlacingPhase:
			// _audio.PlaySFX(SFX.Unit_Click);
				break;

			case GamePhase.CombatPhase:
				if(unit.CanMove && unit.Owner == CurrentPlayer && unit.Owner.CommandPoints > 0){
					HighlightMoveRange(unit);
					// _audio.PlaySFX(SFX.Unit_Click);
				}
				break;

			case GamePhase.TargetPhase:		//This is horribly ineffecitent. Will likely have to store a record of each hero in logic once selected.
				Hero hero = null;
				for(int i = 0; i < players[currentPlayer].army.Count; i++){
					if(players[currentPlayer].army[i].type == UnitType.Hero){
						hero = players[currentPlayer].army[i].GetComponent<Hero>();
					}
				}

				hero.CheckTarget(grid.TileAt(unit.Index));	

				if(hero.targets.Count == hero.currentAbility.targets.Count){
					hero.CastAbility();
					gamePhase = GamePhase.CombatPhase;
				}
				break;
		}
	}

	private void TileLClicked(Tile tile) {
		if(selectedUnit){
			selectedUnit.OnDeselect();
		}

		if(tile.OccupyingUnit){
			UnitSelected(tile.OccupyingUnit);
		}

		switch (gamePhase) {
			case GamePhase.PlacingPhase:
			// _audio.PlaySFX(SFX.Scroll);
				break;

			case GamePhase.CombatPhase:
			// _audio.PlaySFX(SFX.Scroll);
				break;

			case GamePhase.TargetPhase:
				Hero hero = null;
				for(int i = 0; i < players[currentPlayer].army.Count; i++){
					if(players[currentPlayer].army[i].type == UnitType.Hero){
						hero = players[currentPlayer].army[i].GetComponent<Hero>();
					}
				}
				
				hero.CheckTarget(tile);	

				if(hero.targets.Count == hero.currentAbility.targets.Count){
					hero.CastAbility();
					print ("CASTING");
					gamePhase = GamePhase.CombatPhase;
				}

				print("TARGETING COMPLETE!");
				break;
		}
	}

	private void UnitRClicked(Unit unit) {
		switch (gamePhase) {
			case GamePhase.PlacingPhase:
				if (CurrentPlayer.placementField.Contains(unit.Index))
					if (selectedUnit && selectedUnit.Owner == CurrentPlayer)
						if (grid.TileAt(unit.Index).IsPassable){
							SwapUnits(grid.TileAt(unit.Index));
							// _audio.PlaySFX(SFX.Unit_Move);
						}

				break;

			case GamePhase.CombatPhase:
				if (selectedUnit && selectedUnit.Owner == CurrentPlayer && selectedUnit.CanMove && selectedUnit.Owner.CommandPoints > 0){
					if (unit.Owner != CurrentPlayer){
							if (selectedUnit.InAttackRange(unit)){
								if(!selectedUnit.isAttacking && !unit.isAttacking){
									StartCoroutine(UnitCombat(selectedUnit, unit));
									// _audio.PlaySFX(SFX.Rune_Roll);
								}
							}
							else {
								List<Tile> tiles = selectedUnit.TilesReachable();

								Tile closest = null;

								for (int i = 0; i < tiles.Count; i++){
									if(Logic.Inst.Grid.TilesInRange(tiles[i].Index, selectedUnit.attackRange).Contains(Logic.Inst.Grid.TileAt(unit.Index))){
										if(!closest){
											closest = tiles[i];
										}
										else if(Grid.Distance(selectedUnit.Index, tiles[i].Index) <  Grid.Distance(selectedUnit.Index, closest.Index)) {
											closest = tiles[i];	
										}
									}
								}
								
								if(closest){
									selectedUnit.MoveTowardsTile(closest, unit);
								}
							}

					}
				}
				break;
		}
	}

	private void TileRClicked(Tile tile) {
		switch (gamePhase) {
			case GamePhase.PlacingPhase:
				if(CurrentPlayer.placementField.Contains(tile.Index)){
					if (selectedUnit && selectedUnit.Owner == CurrentPlayer && selectedUnit.CanMove)
						if (tile.IsPassable)
							if (!tile.OccupyingUnit)
								selectedUnit.MoveTowardsTile(tile);
							else if (tile.OccupyingUnit.Owner == CurrentPlayer)
								SwapUnits(tile);
				}
				// else
				// _audio.PlaySFX(SFX.Unit_CantMoveThere);
				break;

			case GamePhase.CombatPhase:
				if (selectedUnit && selectedUnit.Owner == CurrentPlayer && selectedUnit.CanMove && selectedUnit.Owner.CommandPoints > 0){
					if (selectedUnit.InMoveRange(tile)) {
						if (!tile.OccupyingUnit)
							if(selectedUnit.CurrentMovePoints > 0){
								selectedUnit.MoveTowardsTile(tile);
								ClearSelected();
							}
						else if (tile.OccupyingUnit.Owner != CurrentPlayer){
							if (!selectedUnit.isAttacking && !tile.OccupyingUnit.isAttacking){
								StartCoroutine(UnitCombat(selectedUnit, tile.OccupyingUnit));
							}
						}
					}
					// else {
					// 	if(tile.OccupyingUnit && tile.OccupyingUnit != selectedUnit.Owner){
					// 		List<Tile> tiles = selectedUnit.TilesReachable();

					// 		Tile closest = null;

					// 		for (int i = 0; i < tiles.Count; i++){
					// 			if(Logic.Inst.Grid.TilesInRange(tiles[i].Index, selectedUnit.attackRange).Contains(Logic.Inst.Grid.TileAt(tile.Index))){
					// 				if(!closest){
					// 					closest = tiles[i];
					// 				}
					// 				else if(Grid.Distance(selectedUnit.Index, tiles[i].Index) <  Grid.Distance(selectedUnit.Index, closest.Index)) {
					// 					closest = tiles[i];	
					// 				}
					// 			}
					// 		}
							
					// 		if(closest){
					// 			selectedUnit.MoveTowardsTile(closest, tile.OccupyingUnit);
					// 		}
					// 	}						
					// }
				}
				break;
		}
	}

	private void SwapUnits(Tile tile) {
		// _audio.PlaySFX(SFX.Unit_Move);
		Tile prevTile = grid.TileAt(selectedUnit.Index);
		Unit swap = tile.OccupyingUnit;
		swap.MoveTowardsTile(prevTile);
		selectedUnit.MoveTowardsTile(tile);
		prevTile.OccupyingUnit = swap;
	}

	public void SetupGameWorld(Army[] armies) {

		// Build grid and set tile modifiers
		grid.GenerateGrid();
		mapGen.GenerateMap(grid.TilesList);

		// Setup inital formations
		formations.Map = grid.TilesList;		
		formations.InitField(armies);

		AssignAltarsInitial();

		gameObject.GetComponent<ClothingManager>().SetSkins();

		for(int i = 0; i < players.Length; i++){
			if(players[i].hero.passive.passive == PassiveType.Buff){
				players[i].hero.passive.ApplyBuffAll(i);
			}
			players[i].hero.gameObject.GetComponent<Unit>().ChangeAnim(0);
		}

		enviro.SetActive(true);
		SwitchGamePhase(GamePhase.PlacingPhase);
	}

	public void StartSetupPhase() {
		GUIManager.inst.GUICanvas.SetActive(true);

		music.ChangeBase(MusicBaseState.Placing);

		currentPlayer = startingPlayer = Random.Range(0, players.Length);
		GUIManager.inst.UpdatePlayerGUI(currentPlayer);

		ChangeTileOutlines(CurrentPlayer.PlacementField(), CurrentPlayer.playerColour, 0.1f);

		Camera.main.GetComponent<Vision>().enabled = true;

		players[currentPlayer].Faith = firstPlayerFaith;
		if(currentPlayer == 0){
			players[1].Faith = secondPlayerFaith;
		}
		else{
			players[0].Faith = secondPlayerFaith;
		}
	}

	public void StartCombatPhase() {
		currentPlayer = startingPlayer;
		GUIManager.inst.UpdatePlayerGUI(currentPlayer);
		music.ChangeBase(MusicBaseState.Battle);
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
					TurnStart();
					return;
				}

				ChangeTileOutlines(CurrentPlayer.PlacementField(), CurrentPlayer.playerColour, 0.1f);
				break;

			case GamePhase.CombatPhase:
				CheckIfPlayerWinning();

				TurnStart();
				
				for(int i = 0; i < grid.TilesList.Count; i++){
					if(CurrentPlayer.hero.type == HeroType.Skadi && grid.TilesList[i].Biome == BiomeType.Snow){
						grid.TilesList[i].MoveCost = 1;
					}
					else if(grid.TilesList[i].Biome == BiomeType.Snow){
						grid.TilesList[i].MoveCost = 2;
					}
				}

				for(int j = 0; j < prevPlayer.army.Count; j++){
					prevPlayer.army[j].OnTurnEnd();
				}
				break;
		}
	}

	public void TurnStart(){
		AddFaithPerAltar();
		CurrentPlayer.StartTurn();
		players[currentPlayer].hero.CalcBuffStrength();
		CurrentPlayer.hero.ApplyPassive();
	}

	public void CheckIfPlayerWinning() {
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

				//timerText.SetActive(true);
				GUIManager.inst.SetPlayerWinning(turnsRemaining);
				//timerText.GetComponent<Text>().text = turnsRemaining + " Turns Until " + players[winningPlayer].name + "'s Victory";

				if (turnsRemaining <= 0)
					EndGame();
			}
			else {
				Debug.Log(players[winning].name + " has taken control of the altars!");
				winningPlayer = winning;
				turnsRemaining = turnsForVictory;
				timerText.SetActive(false);
				GUIManager.inst.SetPlayerWinning(turnsRemaining);
				SetWrathMode();
			}
		}
		else {
			winningPlayer = -1;
			SetWrathMode();
			GUIManager.inst.DisablePlayerWinning();
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
			GUIManager.inst.SetWrathTextOff();
			players[winningPlayer].wrathMode = false;
			for(int i = 0; i < players.Length; i++){
				if(i != winningPlayer){
					players[i].wrathMode = true;
					GUIManager.inst.SetWrathTextOn(i);
				}
			}
		}
		else{
			GUIManager.inst.SetWrathTextOff();
			players[0].wrathMode = false;
			players[1].wrathMode = false;
			turnsRemaining = turnsForVictory;
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
		returnButton.SetActive(true);
		winText.SetActive(true);
		winText.GetComponent<Text>().text = players[winningPlayer].PlayerName + " Wins!";
		for(int i = 0; i < gameGui.Count; i++){
			gameGui[i].SetActive(false);
		}
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

	public IEnumerator UnitCombat(Unit att, Unit def) {
		ClearSelected();
		att.CanMove = false;
		att.CurrentMovePoints = 0;
		att.isAttacking = true;
		def.isAttacking = true;
		combatManager.ResolveCombat(att, def);

		yield return new WaitForSeconds(2.5f);

		att.OnAttack();

		if (!def.dead && def.InAttackRange(att) && !def.hasRetaliated){
			combatManager.ResolveCombat(def, att);
			def.hasRetaliated = true;

			yield return new WaitForSeconds(2.5f);
		}

		att.isAttacking = false;

		if(def){
			def.isAttacking = false;
		}
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

	private void AssignAltarsInitial() {
		for(int i = 0; i < altars.Count; i++){
			bool altarHasOwner = false;
			for(int j = 0; j < Logic.Inst.Players.Length; j++){
				players[j].SetPlayerColor();
				if(Logic.Inst.Players[j].placementField.Contains(altars[i].Index)){
					altarHasOwner = true;
					altars[i].PlayerCaptureAltar(Logic.Inst.Players[j]);
				}
			}
			if(!altarHasOwner){
				altars[i].PlayerCaptureAltar(Logic.Inst.Players[i%Logic.Inst.Players.Length]);
			}	
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

	public Player CurrentWinner {
		get {return players[winningPlayer];}
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

	public MusicPlayer Music {
		get { return music; }
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

	public Combat CombatManager{
		get {return combatManager;}
	}

	public int FaithPerTurn {
		get{ return faithPtsPerAltar * CurrentPlayer.capturedAltars.Count;}
	}
	#endregion
}