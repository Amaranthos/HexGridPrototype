using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour {

	public string playerName = "";
	public int startingFood;
	public int startingFaith;
	public Color playerColour;

	public Field placementBoundaries;

	public bool wrathMode = false;

	public List<Unit> army = new List<Unit>();
	public Hero hero;

	public List<Altar> capturedAltars;

	private int currentFood;
	private int faith;

	public bool Defeated { get; set; }
	private bool FinishedPlacing
		= false;

	private void Awake() {
		Defeated = false;
		currentFood = startingFood;
		faith = startingFaith;
	}

	public void SpawnUnit(UnitType type, Tile tile, int dir) {
		GameObject temp = (GameObject)Instantiate(Logic.Inst.UnitList.GetUnit(type), tile.transform.position, Quaternion.Euler(Vector3.up * (dir * 180 + 90)));
		Unit unit = temp.GetComponent<Unit>();
		unit.Index = tile.Index;
		unit.Owner = this;
		tile.OccupyingUnit = unit;
		army.Add(unit);
		temp.transform.parent = this.transform;
		Logic.Inst.SetUnitMaterial(unit,this);
	}

	public void SpawnHero(Tile tile, int dir) {
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

	public List<Tile> PlacementField() {
		List<Tile> tiles = new List<Tile>();
		foreach(Tile tile in Logic.Inst.Grid.TilesList){
			if (placementBoundaries.CoordsInRange(tile.Index)){
				tiles.Add(tile);
			}
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
		for (int i = 0; i < army.Count; i++) {
			army[i].CanMove = true;
			army[i].ClearHighlightedTiles();
		}

		foreach(Unit unit in army){		//Just for testing. I tried it in the above loop but it was called on every unit mulitple times.
			unit.OnTurnStart();
			
			// Debug.Log(unit.type + " Starting Turn");
		}
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
	#endregion
}
