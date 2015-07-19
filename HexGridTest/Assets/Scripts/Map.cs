using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

	public Mesh sharedMesh;
	public float hexRadius;
	public Vector2 mapSize;
	public int chunkSize;
	public Texture texture;

	Vector3 hexExtents;
	Vector3 hexSize;
	Vector3 hexCentre;

	GameObject chunkHolder;

	int xSectors;
	int zSectors;

	Chunk[,] chunks;


	private void Start() {
		CacheHexProperties();
		GenerateMap();
	}

	void CacheHexProperties() {
		GameObject go = new GameObject();

		MeshFilter filter = go.AddComponent<MeshFilter>();
		MeshCollider collider = go.AddComponent<MeshCollider>();

		go.transform.position = Vector3.zero;
		go.transform.rotation = Quaternion.identity;

		Vector3[] verts;
		int[] tris;
		Vector2[] uvs;

		float floorLevel = 0.0f;

		verts = new Vector3[] {
			new Vector3((hexRadius * Mathf.Cos((float)(2*Mathf.PI*(3+0.5)/6))), floorLevel, (hexRadius * Mathf.Sin((float)(2*Mathf.PI*(3+0.5)/6)))),
            new Vector3((hexRadius * Mathf.Cos((float)(2*Mathf.PI*(2+0.5)/6))), floorLevel, (hexRadius * Mathf.Sin((float)(2*Mathf.PI*(2+0.5)/6)))),
            new Vector3((hexRadius * Mathf.Cos((float)(2*Mathf.PI*(1+0.5)/6))), floorLevel, (hexRadius * Mathf.Sin((float)(2*Mathf.PI*(1+0.5)/6)))),
            new Vector3((hexRadius * Mathf.Cos((float)(2*Mathf.PI*(0+0.5)/6))), floorLevel, (hexRadius * Mathf.Sin((float)(2*Mathf.PI*(0+0.5)/6)))),
            new Vector3((hexRadius * Mathf.Cos((float)(2*Mathf.PI*(5+0.5)/6))), floorLevel, (hexRadius * Mathf.Sin((float)(2*Mathf.PI*(5+0.5)/6)))),
            new Vector3((hexRadius * Mathf.Cos((float)(2*Mathf.PI*(4+0.5)/6))), floorLevel, (hexRadius * Mathf.Sin((float)(2*Mathf.PI*(4+0.5)/6))))
		};

		tris = new int[] {
            1,5,0,
            1,4,5,
            1,2,4,
            2,3,4
        };

		uvs = new Vector2[] {
            new Vector2(0,0.25f),
            new Vector2(0,0.75f),
            new Vector2(0.5f,1),
            new Vector2(1,0.75f),
            new Vector2(1,0.25f),
            new Vector2(0.5f,0),
        };

		sharedMesh = new Mesh();

		sharedMesh.vertices = verts;
		sharedMesh.triangles = tris;
		sharedMesh.uv = uvs;

		filter.mesh = sharedMesh;
		filter.mesh.RecalculateNormals();
		collider.sharedMesh = sharedMesh;

		hexExtents = collider.bounds.extents;
		hexSize = collider.bounds.size;
		hexCentre = collider.bounds.center;

		Destroy(go);

		filter.mesh.RecalculateNormals();
	}

	void GenerateMap() {
		xSectors = Mathf.CeilToInt(mapSize.x / chunkSize);
		zSectors = Mathf.CeilToInt(mapSize.y / chunkSize);

		chunks = new Chunk[xSectors, zSectors];

		for (int x = 0; x < xSectors; x++) {
			for (int z = 0; z < zSectors; z++) {
				chunks[x, z] = NewChunk(x, z);
				chunks[x, z].gameObject.transform.position = new Vector3(x * chunkSize * hexSize.x, 0.0f, z * chunkSize * hexSize.z);
				chunks[x, z].hexSize = hexSize;
				
				chunks[x, z].xSector = x;
				chunks[x, z].zSector = z;
				chunks[x, z].map = this;
			}
		}

		foreach (Chunk chunk in chunks)
			chunk.Create();
	}

	Chunk NewChunk(int x, int z) {
		if (x == 0 && z == 0)
			chunkHolder = new GameObject("Chunks");

		GameObject chunkObj = new GameObject("Chunk[" + x + "," + z + "]");
		Chunk chunk = chunkObj.AddComponent<Chunk>();
		chunk.SetSize(chunkSize, chunkSize);
		chunk.AllocateTileArray();

		chunkObj.AddComponent<MeshRenderer>().material.mainTexture = texture;
		chunkObj.AddComponent<MeshFilter>();
		chunkObj.transform.parent = chunkHolder.transform;

		return chunk;
	}

	public Vector3 TileExtents {
		get { return hexExtents; }
	}

	public Vector3 TileSize {
		get { return hexSize; }
	}

	public Vector3 TileCentre {
		get { return hexCentre; }
	}
}
