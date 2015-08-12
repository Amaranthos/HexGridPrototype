using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	private PairInt index;

	private Unit occupyingUnit = null;

	private bool isPassable = true;

	private float radius;

	public Vector3 Corner(int index) {
		float angle = Mathf.PI / 180 * (60 * index);
		return new Vector3(transform.position.x + radius * Mathf.Cos(angle), 0.0f, transform.position.z + radius * Mathf.Sin(angle));
	}

	public static Vector3 Corner(Vector3 centre, float radius, int index) {
		float angle = Mathf.PI / 180 * (60 * index);
		return new Vector3(centre.x + radius * Mathf.Cos(angle), 0.0f, centre.z + radius * Mathf.Sin(angle));
	}

	public void Start(){

		LineRenderer lines = gameObject.GetComponent<LineRenderer>();
		lines.SetVertexCount(7);

		for (int i = 0; i < 7; i++) {
			lines.SetPosition(i, Corner(i));
		}
	}

	public void SetMesh(Mesh mesh) {
		GetComponent<MeshFilter>().sharedMesh = mesh;
		GetComponent<MeshCollider>().sharedMesh = mesh;
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
	#endregion
}
