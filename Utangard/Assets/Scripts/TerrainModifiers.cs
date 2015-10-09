using UnityEngine;
using System.Collections;

public class TerrainModifiers : MonoBehaviour {
	public static TerrainModifiers inst;

	public TerrainModifier grass;
	public TerrainModifier snow;
	public TerrainModifier forest;

	public TerrainModifier plains;
	public TerrainModifier hills;
	public TerrainModifier mountains;

	public void Awake() {
		if(!inst)
			inst = this;
	}
}

[System.Serializable]
public struct TerrainModifier {
	public int moveCost;
	public int attactEffect;
	public int defenseEffect;	
}