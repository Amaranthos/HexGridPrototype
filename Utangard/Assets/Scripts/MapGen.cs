﻿using UnityEngine;
using System.Collections.Generic;

public class MapGen : MonoBehaviour {

	public string seed;
	public bool useRandomSeed;

	public ProdGenMethod proceduralFillMethod;

	[Range(0,10)]
	public int smoothing;

	[Range(1,10)]
	public int voronoiPoints;	

	[Range(0,50)]
	public int mountainPercent;

	[Range(0,50)]
	public int hillPercent;

	[Range(0,100)]
	public int biomeChangeChance;

	private List<Tile> map;

	private System.Random randGen;

	public void GenerateMap(List<Tile> tiles) {
		if(useRandomSeed){
			seed = System.DateTime.Now.ToString();
		}
		randGen = new System.Random(seed.GetHashCode());

		map = tiles;

		switch(proceduralFillMethod){
			case ProdGenMethod.RandomFill:
				RandomFill();
				break;

			case ProdGenMethod.VoronoiFill:
				VoronoiFill();
				break;
		}

		for(int i = 0; i < smoothing; i++){
			Smooth();
		}

		Altars();
	}

	public void VoronoiFill() {
		List<Tile> centres = new List<Tile>();
	
		int rand;
		BiomeType biome = BiomeType.Grass;
		TerrainType terrain = TerrainType.Plains;

		//Set voronoi points
		for(int i = 0; i < voronoiPoints; i++){
			rand = randGen.Next(0, map.Count);
			if(!centres.Contains(map[rand])){
				biome = RandomBiome();
				map[rand].SetTileType(biome, terrain);
				centres.Add(map[rand]);
			}
			else{
				i--;
			}
		}

		//Find nearest point and set to be the same
		Tile closest = centres[0];
		for(int i = 0; i < map.Count; i++){
			for(int j = 0; j < centres.Count; j++){
				if(map[i] != centres[j]){
					if(Logic.Inst.Grid.Distance(map[i], centres[j]) < Logic.Inst.Grid.Distance(map[i], closest)){
						closest = centres[j];
					}
				}
			}

			//Decide terrain type
			terrain = GenerateTerrain(i);

			if(Logic.Inst.Grid.Distance(map[i], closest) > 5){
				map[i].SetTileType(RandomBiome(), terrain);
			}
			else{
				map[i].SetTileType(closest.Biome, terrain);
			}
		}
	}

	public void RandomFill() {
		TerrainType terrain = TerrainType.Plains;

		for(int i = 0; i < map.Count; i++){
			terrain = GenerateTerrain(i);
			map[i].SetTileType(RandomBiome(), terrain);
		}
	}

	public void Smooth() {
		int length = System.Enum.GetNames(typeof(BiomeType)).Length;
		int[,] count = new int[map.Count, length];

		for(int i = 0; i < map.Count; i++){
			foreach(Tile neighbour in Logic.Inst.Grid.Neighbours(map[i]))
				count[i, (int)neighbour.Biome]++;
		}

		for(int i = 0; i < map.Count; i++){
			int biome = 0;
			int currentBiome = (int) map[i].Biome;

			if(count[i, currentBiome] >= 2){
				biome = currentBiome;
			}
			else{
				for (int j = 0; j < length; j++){
					if(count[i, j] >= 2){
						if(randGen.Next(0, 100) > biomeChangeChance){
							biome = j;
						}
					}
				}
			}
			map[i].SetTileType((BiomeType)biome, map[i].Terrain);		
		}		
	}

	public TerrainType GenerateTerrain(int i) {
		TerrainType terrain = TerrainType.Plains;
		int	rand = randGen.Next(0,200);

		int chance = (int) (mountainPercent * 0.25f * Mathf.Pow(2.0f, Logic.Inst.Grid.Distance(map[i].Index, new CubeIndex(0,0,0))));

		if(rand < chance){
			terrain = TerrainType.Mountains;
		}
		else if(rand > 100 && rand < (hillPercent + 100)){
			terrain = TerrainType.Hills;
		}
		return terrain;
	}

	public void Altars() {
		// Player[]  Logic.Inst.Players = Logic.Inst.Players;
		int altarRatio = Logic.Inst.numAltars /  Logic.Inst.Players.Length;
		
		Debug.Log("Altar ratio: " + altarRatio);

		if(Logic.Inst.numAltars <= Logic.Inst.Altars.Count){
			Debug.Log("Num Altars:" + Logic.Inst.numAltars);
			Debug.Log("Count Altars:" + Logic.Inst.Altars.Count);
			return;
		}

		for(int i = 0; i <  Logic.Inst.Players.Length; i++) {
			Debug.Log("Player: " +  Logic.Inst.Players[i]);
			List<Tile> field =  Logic.Inst.Players[i].PlacementField();

			foreach(Tile tile in field){
				Debug.Log(tile);				
			}

			for(int j = 0; j < altarRatio; j++){
				Tile rand = field[randGen.Next(0, field.Count-1)];

				if(!rand.HasAltar && rand.Terrain == TerrainType.Plains){
					Debug.Log("Tile: " + rand);
					Altar altar = ((GameObject)Instantiate(Logic.Inst.Terrains.GetAltar(), rand.transform.position, Quaternion.Euler(Vector3.up * i * 45))).GetComponent<Altar>();
					altar.Index = rand.Index;
					// altar.transform.parent = rand.transform;
					Logic.Inst.Altars.Add(altar);
					altar.PlayerCaptureAltar( Logic.Inst.Players[i]);
					rand.HasAltar = true;
				}
				else{
					j--;
				}
			}
		}
	}

	public BiomeType RandomBiome(){
		return (BiomeType)(randGen.Next(0,System.Enum.GetNames(typeof(BiomeType)).Length));
	}
}

[System.Serializable]
public enum ProdGenMethod {
	RandomFill,
	VoronoiFill
}