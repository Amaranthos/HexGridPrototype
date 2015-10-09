using UnityEngine;
using System.Collections.Generic;

public class TerrainList : MonoBehaviour {
	public List<GameObject> hills = new List<GameObject>();
	public List<GameObject> mountains = new List<GameObject>();
	public List<GameObject> altars = new List<GameObject>();
	public List<BiomeMat> biomeMaterials = new List<BiomeMat>();

	public GameObject GetHill() {
		return hills[Random.Range(0, hills.Count)];
	}

	public GameObject GetMountain() {
		return mountains[Random.Range(0, mountains.Count)];
	}

	public GameObject GetAltar() {
		return altars[Random.Range(0, altars.Count)];
	}

	public Material GetBiomeMaterial(BiomeType biome) {
		return biomeMaterials.Find(item=>item.biome == biome).mat;
	}
}

[System.Serializable]
public struct BiomeMat{
	public Material mat;
	public BiomeType biome;
}