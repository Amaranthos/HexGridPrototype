using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour {
	public GameObject hexObj;
	public GameObject mountain;

	public PairInt gridSize;
	public float hexRadius;

	private float hexWidth;
	private float hexHeight;
	
	public Tile[,] grid; //odd-q

	private void Start() {
		grid = new Tile[gridSize.x, gridSize.y];
	}

	public void GenerateGrid() {
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

		uvs.Add(new Vector2(0.5f, 1));
		uvs.Add(new Vector2(1, 0.75f));
		uvs.Add(new Vector2(1, 0.25f));
		uvs.Add(new Vector2(0.5f, 0));
		uvs.Add(new Vector2(0, 0.25f));
		uvs.Add(new Vector2(0, 0.75f));

		mesh.vertices = verts.ToArray();
		mesh.triangles = tris.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.RecalculateNormals();

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

	public List<Tile> GetNeighbours(PairInt index) {
		return GetNeighbours(index.x, index.y);
	}

	public List<Tile> GetNeighbours(Tile tile) {
		return GetNeighbours(tile.Index);
	}

	public int Distance(Tile a, Tile b) {
		TripletInt aC = a.CoordsToCubic();
		TripletInt bC = a.CoordsToCubic();
		return (Mathf.Abs(aC.x - bC.x) + Mathf.Abs(aC.y - bC.y) + Mathf.Abs(aC.z - bC.z)) / 2;
	}

	public Tile GetTile(int i, int j) {
		if (i < gridSize.x && i >= 0) {
			if (j < gridSize.y && j >= 0) {
				return grid[i, j];
			}
		}
		return null;
	}

	public Tile GetTile(PairInt index) {
		return GetTile(index.x, index.y);
	}

	public List<Tile> TilesInRange(PairInt index, int range) {
		List<Tile> tiles = new List<Tile>();

		int cost = range;

		for (int i = -cost; i <= cost; i++)
			for (int j = Mathf.Max(-cost, -i - cost); j <= Mathf.Min(cost, -i + cost); j++) {
				PairInt offset = Tile.CubicToIndex(new TripletInt(i, j, -i-j));
				Tile tile = GetTile(index.x + offset.x, index.y + offset.y);

				if(tile && !tile.OccupyngUnit && tile.IsPassable)
					tiles.Add(tile);
			}

		return tiles;
	}

	public void FillBoard() {
		int counter = 0;
		for(int i = 0; i < gridSize.x; i++) {
			for(int j = 0; j < gridSize.y; j++) {
				GameObject temp = (GameObject)Instantiate(Logic.Inst.UnitList.GetUnit(UnitType.Spearman), grid[i,j].transform.position, Quaternion.identity);
				grid[i,j].OccupyngUnit = temp.GetComponent<Unit>();
				counter++;
				if(counter >= 50)
					return;
			}
		}
	}

	#region Getters and Setters
	#endregion

	PairInt[,] directions = new PairInt[,] {{new PairInt(1, 0), new PairInt(1, -1), new PairInt(0, -1), 
												new PairInt(-1, -1), new PairInt(-1, 0), new PairInt(0, 1)}, 
											{new PairInt(1, 1), new PairInt(1, 0), new PairInt(0, -1), 
												new PairInt(-1, 0), new PairInt(-1, 1), new PairInt(0, 1)}};
}
