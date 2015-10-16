using UnityEngine;
using System.Collections.Generic;

public class MapGen : MonoBehaviour {

	public string seed;
	public bool useRandomSeed;

	[Range(0,100)]
	public int mountainPercent;

	[Range(0,100)]
	public int hillPercent;

	private List<Tile> map;

	public void GenerateMap(List<Tile> tiles) {
		map = tiles;

		RandomFill();
		Smooth();
	}

	public void RandomFill() {
		if(useRandomSeed)
			seed = System.DateTime.Now.ToString();

		System.Random randGen = new System.Random(seed.GetHashCode());

		TerrainType terrain = TerrainType.Plains;
		int rand;

		foreach(Tile tile in map){
			terrain = TerrainType.Plains;
			rand = randGen.Next(0,200);
			Debug.Log(rand);

			if(rand < mountainPercent)
				terrain = TerrainType.Mountains;
			else if(rand > 100 && rand < (hillPercent + 100))
				terrain = TerrainType.Hills;

			tile.SetTileType((BiomeType)(randGen.Next(0,System.Enum.GetNames(typeof(BiomeType)).Length)), terrain);
		}
	}

	public void Smooth() {
		int length = System.Enum.GetNames(typeof(BiomeType)).Length;
		int[] count = new int[length * map.Count];

		for(int i = 0; i < map.Count; i++){
			foreach(Tile neighbour in Logic.Inst.Grid.Neighbours(map[i]))
				count[i * 3 + (int)neighbour.Biome]++;
		}

		for(int i = 0; i < map.Count; i++){
			int greatest = 0;
			int biome = 0;

			for (int j = 0; j < length; j++)
				if(count[j + i * 3] > greatest){
					greatest = count[j + i * 3];
					biome = j;
				}

			map[i].SetTileType((BiomeType)biome, map[i].Terrain);		
		}		
	}

	public void Altars() {

	}
}
