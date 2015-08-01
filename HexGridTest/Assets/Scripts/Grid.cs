using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour {
	public GameObject hexObj;

	public PairInt gridSize;
	public Vector2 hexSize;

	public Tile[] grid;

	private void Start() {

		MeshRenderer renderer = hexObj.GetComponent<MeshRenderer>();

		hexSize.x = renderer.bounds.size.x;
		hexSize.y = renderer.bounds.size.y;

		grid = new Tile[gridSize.x * gridSize.y];

		GenerateGrid();
	}

	private void GenerateGrid() {
		for (int i = 0; i < gridSize.x; i++) {
			for (int j = 0; j < gridSize.y; j++) {
				GameObject temp = (GameObject)Instantiate(hexObj, new Vector3(j * hexSize.y + ((i & 1) * 0.5f), 0.0f, i * hexSize.x), Quaternion.identity);
				temp.name = temp.name + "[" + i + "," + j + "]";
				temp.transform.parent = this.transform;
				Tile tile = temp.AddComponent<Tile>();
				tile.Index = new PairInt(i, j);
				grid[i * gridSize.x + j] = tile;
			}
		}
	}

	public List<Tile> GetNeighbours(int x, int y) {
		List<Tile> ret = new List<Tile>();
		Tile temp;

		temp = GetTile(x, y - 1);
		if (temp)
			ret.Add(temp);

		temp = GetTile(x, y + 1);
		if (temp)
			ret.Add(temp);

		temp = GetTile(x + 1, y - 1);
		if (temp)
			ret.Add(temp);

		temp = GetTile(x + 1, y);
		if (temp)
			ret.Add(temp);

		temp = GetTile(x - 1, y - 1);
		if (temp)
			ret.Add(temp);

		temp = GetTile(x - 1, y);
		if (temp)
			ret.Add(temp);

		return ret;
	}

	public Tile GetTile(int i, int j) {
		if (i < gridSize.x && i >= 0) {
			if (j < gridSize.y && j >= 0) {
				return grid[i * gridSize.x + j];
			}
		}
		return null;
	}

	#region Getters and Setters
	#endregion
}
