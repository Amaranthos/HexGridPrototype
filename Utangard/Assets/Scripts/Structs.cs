using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//[System.Serializable]
//public struct PairInt {
//	public int x;
//	public int y;
//
//	public PairInt(int x, int y) {
//		this.x = x;
//		this.y = y;
//	}
//
//	public static bool operator ==(PairInt lhs, PairInt rhs) {
//		return (lhs.x == rhs.x) && (lhs.y == rhs.y);
//	}
//
//	public static bool operator !=(PairInt lhs, PairInt rhs) {
//		return (lhs.x != rhs.x) || (lhs.y != rhs.y);
//	}
//}
//
//[System.Serializable]
//public struct TripletInt {
//	public int x;
//	public int y;
//	public int z;
//
//	public TripletInt(int x, int y, int z) {
//		this.x = x;
//		this.y = y;
//		this.z = z;
//	}
//}

[System.Serializable]
public struct StatsGUI{
	public Text hp;
	public Text moves;
	public Text atk;
	public Text def;
	public Text dodge;
}

[System.Serializable]
public struct Field {
	public int x;
	public int w;

	public Field(int x, int w) {
		this.x = x;
		this.w = w;
	}

	public bool CoordsInRange(int i) {
		return (i >= x && i < x + w);
	}

	public bool CoordsInRange(CubeIndex coords) {
		return CoordsInRange(coords.x);
	}
}
