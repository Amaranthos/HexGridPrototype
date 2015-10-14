﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CSVParser{
	public string GetCSV(string filePath)
	{
		string fileData = System.IO.File.ReadAllText(filePath);
		return fileData;
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