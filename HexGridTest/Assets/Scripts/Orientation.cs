using UnityEngine;
using System.Collections;

public struct Orientation {
	public readonly Vector4 f;
	public readonly Vector4 b;
	public readonly float startAngle;

	static public Orientation Pointy = new Orientation(new Vector4(Mathf.Sqrt(3.0f), Mathf.Sqrt(3.0f) / 2.0f, 0.0f, 3.0f / 2.0f), new Vector4(Mathf.Sqrt(3.0f) / 3.0f, -1.0f / 3.0f, 0.0f, 2.0f / 3.0f), 0.5f);
	static public Orientation Flat = new Orientation(new Vector4 (3.0f / 2.0f, 0.0f, Mathf.Sqrt(3.0f) / 2.0f, Mathf.Sqrt(3.0f)), new Vector4 (2.0f / 3.0f, 0.0f, -1.0f / 3.0f, Mathf.Sqrt(3.0f) / 3.0f), 0.0f);

	Orientation(Vector4 f, Vector4 b, float startAngle) {
		this.f = f;
		this.b = b;
		this.startAngle = startAngle;
	}
}