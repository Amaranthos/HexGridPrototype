using UnityEngine;
using System.Collections.Generic;

public class Formations : MonoBehaviour {

	public UnitFormations form;

	private List<Tile> map;
	private List<CubeIndex> p1Indexes = new List<CubeIndex>();
	private List<CubeIndex> p2Indexes = new List<CubeIndex>();

	private int startColumn = -1;

	public void InitField(Army[] armies) {
		// Setup placement fields
		List<Tile> tiles = map.FindAll(item=>item.Index.x <= startColumn);

		for(int i = 0; i < tiles.Count; i++){
			p1Indexes.Add (tiles[i].Index);
		}

		for(int i = 0; i < tiles.Count; i++){
			// Player 2's placement field is rotated pi radians
			p2Indexes.Add (new CubeIndex(-p1Indexes[i].x, -p1Indexes[i].y, -p1Indexes[i].z));
		}

		Logic.Inst.Players[0].placementField = p1Indexes;
		Logic.Inst.Players[1].placementField = p2Indexes;

		// Get the appropriate formation and spawn units
		for(int i = 0; i < Logic.Inst.Players.Length; i++){
			Logic.Inst.Players[i].units = armies[i];
			var formation = GetFormation(i, armies[i]);

			for(int j = 0; j < formation.Count; j++){
				if(formation[j].First != UnitType.Hero){
					Logic.Inst.Players[i].SpawnUnit(formation[j].First, Logic.Inst.Grid.TileAt(formation[j].Second), i);
				}
				else {
					Logic.Inst.Players[i].SpawnHero(Logic.Inst.Grid.TileAt(formation[j].Second), i);	
				}
			}			
		}
	}

	public List<Pair<UnitType, CubeIndex>> GetFormation(int player, Army troops){
		var formation = FormPositions(troops);

		// Rotate formation for player 2
		if(player == 1){
			for(int i = 0; i < formation.Count; i++){
				formation[i].Second = new CubeIndex(-formation[i].Second.x, -formation[i].Second.y, -formation[i].Second.z);
			}
		}
		return formation;
	}

	public List<Pair<UnitType, CubeIndex>> FormPositions(Army troops) {
		switch(form){
			case UnitFormations.Aggressive:
				return AggressiveFormation(troops);

			case UnitFormations.Defensive:
				return DefensiveFormation(troops);

			case UnitFormations.Skirmish:
				return SkirmishFormation(troops);
		}
		return null;
	}
		
	public List<Pair<UnitType, CubeIndex>> AggressiveFormation(Army troops){
		var formation = new List<Pair<UnitType, CubeIndex>>();

		int x = startColumn; // Start column
		int z = 0;
		int c = 0;

		for(int i = 0; i < troops.axes; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Axemen, new CubeIndex(x,z)));
		}

		for(int i = 0; i < troops.spears; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Spearman, new CubeIndex(x,z)));
		}

		x -= 1;
		z = Mathf.Abs(x)/2;
		c = 1;

		formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Hero, new CubeIndex(x,z)));

		for(int i = 0; i < troops.swords; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Swordsmen, new CubeIndex(x,z)));
		}
		return formation;
	}

	public List<Pair<UnitType, CubeIndex>> DefensiveFormation(Army troops){
		var formation = new List<Pair<UnitType, CubeIndex>>();

		int x = startColumn; // Start column
		int z = 0;
		int c = 0;

		for(int i = 0; i < troops.swords; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Swordsmen, new CubeIndex(x,z)));
		}

		x -= 1;
		z = Mathf.Abs(x)/2;
		c = 0;

		for(int i = 0; i < troops.spears; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Spearman, new CubeIndex(x,z)));
		}

		for(int i = 0; i < troops.axes; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Axemen, new CubeIndex(x,z)));
		}

		x -= 1;
		z = Mathf.Abs(x)/2;
		c = 1;

		formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Hero, new CubeIndex(x,z)));

		return formation;
	}

	public List<Pair<UnitType, CubeIndex>> SkirmishFormation(Army troops){
		var formation = new List<Pair<UnitType, CubeIndex>>();

		int x = startColumn; // Start column
		int z = 0;
		int c = 0;

		for(int i = 0; i < troops.axes; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Axemen, new CubeIndex(x,z)));
		}

		for(int i = 0; i < troops.spears; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Spearman, new CubeIndex(x,z)));
		}

		x -= 1;
		z = Mathf.Abs(x)/2;
		c = 1;

		formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Hero, new CubeIndex(x,z)));

		for(int i = 0; i < troops.swords; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Swordsmen, new CubeIndex(x,z)));
		}
		return formation;
	}

	public List<Tile> Map {
		set {map = value;}
	}
}