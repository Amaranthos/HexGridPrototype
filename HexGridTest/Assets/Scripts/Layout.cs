using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Layout {
	public readonly Orientation orientation;
	public readonly Vector2 size;
	public readonly Vector2 origin;

	public Layout(Orientation orientation, Vector2 size, Vector2 origin) {
		this.orientation = orientation;
		this.size = size;
		this.origin = origin;
	}

	static public Vector2 HexToPixel(Layout layout, Hex h) {
		Orientation o = layout.orientation;
		Vector2 size = layout.size;
		Vector2 origin = layout.origin;
		float x = (o.f.x * h.q + o.f.y * h.r) * size.x;
		float y = (o.f.z * h.q + o.f.w * h.r) * size.y;
		return new Vector2(x + origin.x, y + origin.y);
	}

	static public FractionalHex PixelToHex(Layout layout, Vector2 point) {
		Orientation o  = layout.orientation;
		Vector2 pt = new Vector2((point.x - layout.origin.x) / layout.size.x, (point.y - layout.origin.y) / layout.size.y);
		float q = o.b.x * point.x + o.b.y * pt.y;
		float r = o.b.z * point.x + o.b.w * pt.y;
		return new FractionalHex(q, r, -q-r);
	}

	static public Vector2 HexCornerOffset(Layout layout, int corner) {
		Orientation o = layout.orientation;
		Vector2 size = layout.size;
		float angle = 2.0f * Mathf.PI * (corner + o.startAngle) / 6;
		return new Vector2(size.x * Mathf.Cos(angle), size.y * Mathf.Sin(angle));
	}

	static public List<Vector2> PolygonCorners(Layout layout, Hex hex) {
		List<Vector2> corners = new List<Vector2>();
		Vector2 centre = HexToPixel(layout, hex);

		for (int i = 0; i < 6; i++) {
			Vector2 offset = HexCornerOffset(layout, i);
			corners.Add(new Vector2(centre.x + offset.x, centre.y + offset.y));
		}

		return corners;
	}
}