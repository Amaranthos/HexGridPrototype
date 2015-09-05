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

	public static bool operator ==(PairInt lhs, PairInt rhs) {
		return (lhs.x == rhs.x) && (lhs.y == rhs.y);
	}

	public static bool operator !=(PairInt lhs, PairInt rhs) {
		return (lhs.x != rhs.x) || (lhs.y != rhs.y);
	}
}

[System.Serializable]
public struct TripletInt {
	public int x;
	public int y;
	public int z;

	public TripletInt(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
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
