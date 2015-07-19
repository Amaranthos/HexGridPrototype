using UnityEngine;
using System.Collections;

public class Tile {

	Vector3 index;
	Vector3 worldPos;
	Vector3 localPos;

	Vector3 extents;
	Vector3 centre;

	Chunk parent;
	Mesh mesh;

	Vector3[] verts;
	int[] tris;
	Vector3[] uvs;

	public void Create() {
		mesh = new Mesh();

		mesh = parent.map.sharedMesh;
		mesh.RecalculateNormals();
	}

	public Vector3 Index {
		get { return index; }
		set { index = value; }
	}

	public Vector3 WorldPos {
		get { return worldPos; }
		set { worldPos = value; }
	}

	public Vector3 LocalPos {
		get { return localPos; }
		set { localPos = value; }
	}

	public Vector3 Extents {
		get { return extents; }
		set { extents = value; }
	}

	public Vector3 Centre {
		get { return centre; }
		set { centre = value; }
	}

	public Chunk Parent {
		get { return parent; }
		set { parent = value; }
	}

	public Mesh Mesh {
		get { return mesh; }
		set { mesh = value; }
	}
}
