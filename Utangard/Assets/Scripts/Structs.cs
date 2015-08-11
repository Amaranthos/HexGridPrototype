using UnityEngine;
using System.Collections;

[System.Serializable]
public struct PairInt {
	public int x;
	public int y;

	public PairInt(int x, int y) {
		this.x = x;
		this.y = y;
	}
}

[System.Serializable]
public struct Rect {
	public int x;
	public int w;
	public int y;
	public int h;

	public Rect(int x, int w, int y, int h) {
		this.x = x;
		this.w = w;
		this.y = y;
		this.h = h;
	}

	public bool CoordsInRange(int i, int j) {
		return (i >= x && i < x + w && j >= y && j < y + h);
	}

	public bool CoordsInRange(PairInt coords) {
		return CoordsInRange(coords.x, coords.y);
	}
}
