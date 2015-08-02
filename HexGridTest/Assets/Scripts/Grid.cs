using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour {
	public GameObject hexObj;

	public PairInt gridSize;
	public float hexRadius;

	private float hexWidth;
	private float hexHeight;
	
	public Tile[,] grid;

	private void Start() {
		grid = new Tile[gridSize.x, gridSize.y];

		GenerateGrid();
	}

	private void GenerateGrid() {
		hexWidth = hexRadius * 2;
		hexHeight = (Mathf.Sqrt(3) / 2) * hexWidth;

		Mesh mesh = new Mesh();

		List<Vector3> verts = new List<Vector3>();
		List<int> tris = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		for(int i = 0; i < 6; i++)
			verts.Add(Tile.Corner(Vector3.zero, hexRadius, i));

		tris.Add(0);
		tris.Add(2);
		tris.Add(1);

		tris.Add(0);
		tris.Add(5);
		tris.Add(2);

		tris.Add(2);
		tris.Add(5);
		tris.Add(3);

		tris.Add(3);
		tris.Add(5);
		tris.Add(4);

		mesh.vertices = verts.ToArray();
		mesh.triangles = tris.ToArray();

		for (int i = 0; i < gridSize.x; i++) {
			for (int j = 0; j < gridSize.y; j++) {
				GameObject go = (GameObject)Instantiate(hexObj, Vector3.zero, Quaternion.identity);
				go.transform.position = new Vector3(i * hexWidth * 3 / 4, 0.0f, j * hexHeight + ((i & 1) * 0.5f * hexHeight));
				go.name = "Hex [" + i + "," + j + "]";
				go.transform.parent = this.transform;
				
				Tile tile = go.GetComponent<Tile>();
				tile.Radius = hexRadius;
				tile.Index = new PairInt(i, j);
				tile.SetMesh(mesh);

				grid[i, j] = tile;
			}
		}

		for (int i = 0; i < gridSize.x; i++) {
			for (int j = 0; j < gridSize.y; j++) {
				grid[i, j].neighbours.AddRange(GetNeighbours(i,j));
			}
		}
	}

	public List<Tile> GetNeighbours(int x, int y) {
		List<Tile> ret = new List<Tile>();

		int parity = x & 1;

		for (int i = 0; i < 6; i++) {
			PairInt dir = directions[parity, i];
			ret.Add(GetTile(x + dir.x, y + dir.y));
		}
		return ret;
	}

	public Tile GetTile(int i, int j) {
		if (i < gridSize.x && i >= 0) {
			if (j < gridSize.y && j >= 0) {
				return grid[i, j];
			}
		}
		return null;
	}

	#region Getters and Setters
	#endregion

	PairInt[,] directions = new PairInt[,] {{new PairInt(1, 0), new PairInt(1, -1), new PairInt(0, -1), 
												new PairInt(-1, -1), new PairInt(-1, 0), new PairInt(0, 1)}, 
											{new PairInt(1, 1), new PairInt(1, 0), new PairInt(0, -1), 
												new PairInt(-1, 0), new PairInt(-1, 1), new PairInt(0, 1)}};
}
