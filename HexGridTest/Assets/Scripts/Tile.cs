using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	private PairInt index;
	private Unit occupyingUnit = null;

	private float radius;

	public List<Tile> neighbours = new List<Tile>();

	private void OnMouseUp() {
		Logic.Inst.TileClicked(this);
	}

	private Vector3 Corner(int index) {
		float angle = Mathf.PI / 180 * (60 * index);
		return new Vector3(transform.position.x + radius * Mathf.Cos(angle), 0.0f, transform.position.z + radius * Mathf.Sin(angle));
	}

	public void Start(){

		Mesh mesh = new Mesh();



		LineRenderer lines = gameObject.GetComponent<LineRenderer>();
		lines.SetVertexCount(7);

		for (int i = 0; i < 7; i++) {
			lines.SetPosition(i, Corner(i));
		}
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
	#endregion
}
