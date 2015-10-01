using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	private PairInt index;

	private Unit occupyingUnit = null;

	private bool isPassable = true;

	private float radius;
	private int cost;

	private LineRenderer lines;

	public Vector3 Corner(int index) {
		float angle = Mathf.PI / 180 * (60 * index);
		return new Vector3(transform.position.x + radius * Mathf.Cos(angle), 0.0f, transform.position.z + radius * Mathf.Sin(angle));
	}

	public static Vector3 Corner(Vector3 centre, float radius, int index) {
		float angle = Mathf.PI / 180 * (60 * index);
		return new Vector3(centre.x + radius * Mathf.Cos(angle), 0.0f, centre.z + radius * Mathf.Sin(angle));
	}

	public void Awake() {
		lines = gameObject.GetComponentInChildren<LineRenderer>();

		if (!lines)
			Debug.LogError("Line renderer component not added to tile!");
	}

	public void Start(){		
		lines.SetVertexCount(7);

		for (int i = 0; i < 7; i++) {
			lines.SetPosition(i, Corner(i));
		}
	}

	public void SetMesh(Mesh mesh) {
		GetComponent<MeshFilter>().sharedMesh = mesh;
		GetComponent<MeshCollider>().sharedMesh = mesh;
	}

	public void SetLineColour(Color colour) {
		lines.SetColors(colour, colour);
	}

	public void SetWidth(float thickness) {
		lines.SetWidth(thickness, thickness);
	}

//	public TripletInt CoordsToCubic() {
//		int x = index.x;
//		int z = index.y - (index.x - (index.x&1))/2;
//		int y = -x-z;
//		return new TripletInt(x,y,z);
//	}

	public TripletInt CoordsToCubic() {
		int x = index.x;
		int z = index.y - (index.x + (index.x&1))/2;
		int y = -x-z;
		return new TripletInt(x,y,z);
	}
	
	public static PairInt CubicToIndex(TripletInt cubic) {
		int col = cubic.x;
		int row = cubic.z + (cubic.x + (cubic.x&1))/2;
		return new PairInt(col, row);
	}

	#region Getters and Setters
	public PairInt Index {
		get { return index; }
		set { index = value; }
	}

	public Unit OccupyngUnit {
		get { return occupyingUnit; }
		set { occupyingUnit = value; }
	}

	public float Radius {
		get { return radius; }
		set { radius = value; }
	}

	public bool IsPassable {
		get { return isPassable; }
		set { isPassable = value; }
	}

	public int MoveCost { get; set; }
	public int GCost { get; set; }
	public int HCost { get; set; }
	public int FCost { get { return GCost + HCost; } }
	public Tile Parent { get; set; }
	#endregion
}
