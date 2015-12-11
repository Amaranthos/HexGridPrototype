using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CSVParser{
	public string GetCSV(string filePath)
	{
		TextAsset data = Resources.Load (filePath) as TextAsset;
		return data.text;
	}
}

[System.Serializable]
public class Target{
	public Unit unit;
	public CubeIndex Index, origin;
	public TargetType type;
	public bool needsUnit;
	public bool needsSpace;

	public Target (Unit unt, CubeIndex pint, TargetType tt, bool nu, bool ns){
		unit = unt;
		Index = pint;
		type = tt;
		needsUnit = nu;
		needsSpace = ns;
	}
}

[System.Serializable]
public class TextSpawn{
	public Buff buff;
	public Unit unit;
	public bool add;

	public TextSpawn(Buff bff, Unit unt, bool ad){
		buff = bff;
		unit = unt;
		add = ad;
	}
}