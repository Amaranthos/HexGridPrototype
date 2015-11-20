using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour {

	public string playerName = "";
	public int startingFood;
	public int startingFaith;
	public int startingCommandPoints;
	public Color playerColour;

	public List<CubeIndex> placementField;

	public bool wrathMode = false;

	public List<Unit> army = new List<Unit>();
	public Army units;
	public Hero hero;

	public List<Altar> capturedAltars;

	private int currentFood;
	private int currentCP;
	private int faith;

	public bool Defeated { get; set; }
	private bool FinishedPlacing = false;

	private void Awake() {
		Defeated = false;
		currentFood = startingFood;
		faith = startingFaith;
		currentCP = startingCommandPoints;
	}

	public void SpawnUnit(UnitType type, Tile tile, int dir) {
		tile.SetTileType(tile.Biome, TerrainType.Plains);

		GameObject temp = (GameObject)Instantiate(Logic.Inst.UnitList.GetUnit(type), tile.transform.position, Quaternion.Euler(Vector3.up * (dir * 180 + 90)));
		Unit unit = temp.GetComponent<Unit>();
		unit.Index = tile.Index;
		unit.Owner = this;
		tile.OccupyingUnit = unit;
		army.Add(unit);
		temp.transform.parent = this.transform;
	}

	public void SpawnHero(Tile tile, int dir) {
		tile.SetTileType(tile.Biome, TerrainType.Plains);
		
		GameObject temp = (GameObject)Instantiate(Logic.Inst.HeroList.GetHero(hero.type), tile.transform.position, Quaternion.Euler(Vector3.up * (dir * 180 + 90)));
		Unit unit = temp.GetComponent<Unit>();
		unit.Index = tile.Index;
		unit.Owner = this;
		tile.OccupyingUnit = unit;
		army.Add(unit);
		temp.transform.parent = this.transform;
		Hero h = temp.GetComponent<Hero>();
		h.hero = unit;
		hero = h;
	}

	public void ClearTroops(){
		for(int i = army.Count; i > 0; i--){
			GameObject.Destroy(army[i]);
		}
	}

	public List<Tile> PlacementField() {
		List<Tile> tiles = new List<Tile>();
		for(int i = 0; i < placementField.Count; i++){
			tiles.Add(Logic.Inst.Grid.TileAt(placementField[i]));
		}
		return tiles;
	}

	public void RemoveUnit(Unit unit) {
		if (army.Contains(unit))
			army.Remove(unit);
	}

	public void StartPlacing() {

	}

	public void StartTurn() {
		currentCP = startingCommandPoints;

		for (int i = 0; i < army.Count; i++) {
			army[i].CanMove = true;
			army[i].ClearHighlightedTiles();
		}

		for(int j = 0; j < army.Count; j++){		//Just for testing. I tried it in the above loop but it was called on every unit mulitple times.
			army[j].OnTurnStart();
			
			// Debug.Log(unit.type + " Starting Turn");
		}
	}

	public void SetPlayerColor(){
		List<Color> cols = GameObject.Find("Manager").GetComponent<HeroList>().heroColors;
		switch(hero.type){
		case HeroType.Eir:
			playerColour = cols[0];
			break;
		case HeroType.Heimdal:
			playerColour = cols[1];
			break;
		case HeroType.Skadi:
			playerColour = cols[2];
			break;
		case HeroType.Thor:
			playerColour = cols[3];
			break;
		default:
			print("Is this even a hero?");
			break;
		}
	}

	public void ClearFormation() {
		foreach(Unit unit in army){
			Destroy(unit.gameObject);
		}

		army.Clear ();
	}

	#region Getters and Setters
	public string PlayerName {
		get { return playerName; }
	}

	public int CurrentFood {
		get { return currentFood; }
		set { currentFood = value; }
	}

	public bool HasFinishedPlacing {
		get { return FinishedPlacing; }
		set { FinishedPlacing = value; }
	}

	public int Faith {
		get { return faith; }
		set { faith = value; }
	}

	public int CommandPoints {
		get {return currentCP;}
		set {currentCP = value;}
	}
	#endregion
}
