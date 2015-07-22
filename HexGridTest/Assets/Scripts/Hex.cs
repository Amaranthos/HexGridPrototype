using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct Hex {
	static public List<Hex> directions = new List<Hex> { new Hex(1, 0, -1), new Hex(1, -1, 0), new Hex(0, -1, 1), new Hex(-1, 0, 1), new Hex(-1, 1, 0), new Hex(0, 1, -1) };

	public readonly int q, r, s;

	public Hex(int q, int r, int s) {
		this.q = q;
		this.r = r;
		this.s = s;
	}

	public Hex(int q, int r) {
		this.q = q;
		this.r = r;
		this.s = -q - r;
	}

	static public bool operator ==(Hex a, Hex b) {
		return a.q == b.q && a.r == b.r && a.s == b.s;
	}

	static public bool operator !=(Hex a, Hex b) {
		return !(a == b);
	}

	static public Hex operator +(Hex a, Hex b) {
		return new Hex(a.q + b.q, a.r + b.r, a.s + b.s);
	}

	static public Hex operator -(Hex a, Hex b) {
		return new Hex(a.q - b.q, a.r - b.r, a.s - b.s);
	}

	static public Hex operator *(Hex a, int b) {
		return new Hex(a.q * b, a.r + b, a.s + b);
	}

	public override int GetHashCode() {
		int hash = 17;
		hash *= 23 + q.GetHashCode();
		hash *= 23 + r.GetHashCode();
		return hash;
	}

	public override bool Equals(object obj) {
		if (obj == null) return false;
		if (GetType() != obj.GetType()) return false;
		Hex other = (Hex)obj;
		return (this.q == other.q) && (this.r == other.r);
	}

	static public int Length(Hex hex) {
		return (int)((Math.Abs(hex.q) + Math.Abs(hex.r) + Math.Abs(hex.s)) / 2);
	}

	static public int Distance(Hex a, Hex b) {
		return Length(a - b);
	}

	static public Hex Direction(int direction) {
		return directions[direction];
	}

	static public Hex Neighbour(Hex hex, int direction) {
		return hex + Direction(direction);
	}

	static public Vector3 CubeCoords(int q, int r) {
		return new Vector3(q, 0, r + (q - (q & 1)) / 2);
	}
}

public struct FractionalHex {

	public readonly float q, r, s;

	public FractionalHex(float q, float r, float s) {
		this.q = q;
		this.r = r;
		this.s = s;
	}

	static public Hex HexRound(FractionalHex hex) {
		int q = (int)(Mathf.Round(hex.q));
		int r = (int)(Mathf.Round(hex.r));
		int s = (int)(Mathf.Round(hex.s));

		float qDiff = Mathf.Abs(q - hex.q);
		float rDiff = Mathf.Abs(r - hex.r);
		float sDiff = Mathf.Abs(s - hex.s);

		if(qDiff > rDiff && qDiff > sDiff) q = -r -s;
		else {
			if(rDiff > sDiff) r = -q-s;
			else s=-q-r;
		}
		return new Hex(q,r,s);
	}

	static public FractionalHex HexLerp(Hex a, Hex b, float t) {
		return new FractionalHex(a.q + (b.q - a.q) * t, a.r + (b.r - a.r) * t, a.s + (b.s - a.s) * t);
	}

	static public List<Hex> HexLineDraw(Hex a, Hex b) {
		int n = Hex.Distance(a, b);
		List<Hex> results = new List<Hex>();
		float step = 1.0f / Mathf.Max(n, 1);

		for (int i = 0; i < n; i++) 
			results.Add(HexRound(HexLerp(a, b, step * i)));

		return results;
	}
}