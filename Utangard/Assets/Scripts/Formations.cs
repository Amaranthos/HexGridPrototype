using UnityEngine;
using System.Collections.Generic;

public class Formations : MonoBehaviour {

	public UnitFormations form;

	private List<Tile> map;

	public void SpawnTroops(Player player, int[] troops, int dir) {
		List<Tile> positions = FormPositions(troops);

		for(int i = 0; i < troops.Length; i++){
			player.SpawnUnit((UnitType)troops[i], positions[i], dir);
		}

		Frontline();
	}

	public List<Tile> FormPositions(int[] troops) {

		List<Tile> ret = new List<Tile>();

		switch(form){
			case UnitFormations.Aggressive:
				break;

			case UnitFormations.Defensive:
				break;

			case UnitFormations.Skirmish:
				break;
		}

		return ret;
	}

	private List<Tile> Frontline () {
		List<Tile> ret = map.FindAll(item=>item.Index.x == 1);

		for(int i = 0; i < ret.Count; i++){
			ret[i].LineColour(Color.magenta);
		}

		return ret;
	}

	public List<Tile> Map {
		set {map = value;}
	}
}