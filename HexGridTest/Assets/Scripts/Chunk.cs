using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour {
	public Tile[,] tiles;
	public int xSize;
	public int zSize;
	public Vector3 hexSize;

	public int xSector;
	public int zSector;

	public Map map;

	MeshFilter filter;
	MeshCollider collider;
	MeshRenderer renderer;

	public void SetSize(int x, int z) {
		xSize = x;
		zSize = z;
	}

	public void OnDestroy() {
		//Destroy(renderer.material);
	}

	public void AllocateTileArray() {
		tiles = new Tile[xSize, zSize];
	}

	public void Create() {
		filter = GetComponent<MeshFilter>();
		collider = GetComponent<MeshCollider>();
		renderer = GetComponent<MeshRenderer>();

		Generate();
		for (int i = 0; i < xSize; i++) {
			for (int j = 0; j < zSize; j++) {
				if (tiles[i, j] == null) {
					tiles[i, j].Parent = this;
					tiles[i, j].Create();
				}
			}
		}
		Combine();
	}

	private void Generate() {
		bool odd;

		for (int i = 0; i < zSize; i++) {
			odd = (i % 2) == 0;
			if (odd)
				for (int j = 0; j < xSize; j++)
					NewTile(i, j);
			else
				for (int j = 0; j < xSize; j++)
					NewOffsetTile(i, j);
		}
	}

	private void NewTile(int x, int z) {
		Tile tile;

		Vector2 index;
		tiles[x, z] = tile = new Tile();

		index.x = x + (xSize * xSector);
		index.y = z + (zSize * zSector);

		tile.Index = new Vector3(index.x - Mathf.Round((index.y / 2) + 0.1f), index.y, -(index.x - Mathf.Round((index.y / 2) + 0.1f) + index.y));
		tile.LocalPos = new Vector3(x * (map.TileExtents.x * 2), 0, (z * map.TileExtents.z) * 1.5f);
		tile.WorldPos = new Vector3(tile.LocalPos.x + (xSector * xSize * hexSize.x), tile.LocalPos.y, tile.LocalPos.z + ((zSector * zSize * hexSize.z) * 0.75f));

		tile.Extents = map.TileExtents;
		tile.Centre = map.TileCentre;
	}

	private void NewOffsetTile(int x, int z) {
		Tile tile;

		Vector2 index;
		tiles[x, z] = tile = new Tile();

		index.x = x + (xSize * xSector);
		index.y = z + (zSize * zSector);

		tile.Index = new Vector3(index.x - Mathf.Round((index.y / 2) + 0.1f), index.y, -(index.x - Mathf.Round((index.y / 2) + 0.1f) + index.y));
		tile.LocalPos = new Vector3(x * (map.TileExtents.x * 2) + map.TileExtents.x, 0, (z * map.TileExtents.z) * 1.5f);
		tile.WorldPos = new Vector3(tile.LocalPos.x + (xSector * xSize * hexSize.x), tile.LocalPos.y, tile.LocalPos.z + ((zSector * zSize * hexSize.z) * 0.75f));

		tile.Extents = map.TileExtents;
		tile.Centre = map.TileCentre;
	}

	private void Combine() {
		CombineInstance[,] combine = new CombineInstance[xSize, zSize];
		for (int i = 0; i < xSize; i++) {
			for (int j = 0; j < zSize; j++) {
				combine[i, j].mesh = tiles[i, j].Mesh;
				Matrix4x4 mat = new Matrix4x4();
				mat.SetTRS(tiles[i, j].LocalPos, Quaternion.identity, Vector3.one);
				combine[i, j].transform = mat;
			}
		}
		filter.mesh = new Mesh();

		CombineInstance[] final;

		List<CombineInstance> list = new List<CombineInstance>();

		foreach (CombineInstance c in combine)
			list.Add(c);

		final = list.ToArray();

		filter.mesh.CombineMeshes(final);
		filter.mesh.RecalculateNormals();
		filter.mesh.RecalculateBounds();
		
	}
}
