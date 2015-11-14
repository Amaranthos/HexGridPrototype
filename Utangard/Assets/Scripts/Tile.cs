using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour, IBinaryHeapItem<Tile> {
	public CubeIndex index;

	private Unit occupyingUnit = null;
	private bool isPassable = true;

	private BiomeType biome = BiomeType.Grass;
	private TerrainType terrain = TerrainType.Plains;

	private GameObject terrainPiece;

	public bool hasAltar = false;

	private int moveCost = 0;

	public static Vector3 Corner(Vector3 origin, float radius, int corner, HexOrientation orientation){
		float angle = 60 * corner;
		if(orientation == HexOrientation.Pointy)
			angle += 30;
		angle *= Mathf.PI / 180;
		return new Vector3(origin.x + radius * Mathf.Cos(angle), 0.0f, origin.z + radius * Mathf.Sin(angle));
	}

	public static void GetHexMesh(float radius, HexOrientation orientation, ref Mesh mesh) {
		mesh = new Mesh();

		List<Vector3> verts = new List<Vector3>();
		List<int> tris = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		for (int i = 0; i < 6; i++){
			verts.Add(Corner(Vector3.zero, radius, i, orientation));
		}

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

		for (int i = 0; i < 6; i++){
			Vector3 corner = Corner(Vector3.zero, radius, i, orientation);
			uvs.Add(new Vector2(corner.x, corner.z));
		}

		mesh.vertices = verts.ToArray();
		mesh.triangles = tris.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.name = "Hexagonal Plane";

		mesh.RecalculateNormals();
	}

	public void SetTileType(BiomeType biome, TerrainType terrain){
		if(biome == BiomeType.Forest){
			terrain = TerrainType.Plains;
		}

		this.biome = biome;
		this.terrain = terrain;
		moveCost = 0;
		isPassable = true;

		if(terrainPiece){
			GameObject.Destroy(terrainPiece);
		}

		switch(this.biome){
		case BiomeType.Grass:
			moveCost = Mathf.Max(moveCost, TerrainModifiers.inst.grass.moveCost);
			break;

		case BiomeType.Snow:
			moveCost = Mathf.Max(moveCost, TerrainModifiers.inst.snow.moveCost);
			break;

		case BiomeType.Forest:
			moveCost = Mathf.Max(moveCost, TerrainModifiers.inst.forest.moveCost);
			break;
		}

		switch (this.terrain){
		case TerrainType.Plains:
			moveCost = Mathf.Max(moveCost, TerrainModifiers.inst.plains.moveCost);	  
			break;

		case TerrainType.Hills:
			moveCost = Mathf.Max(moveCost, TerrainModifiers.inst.hills.moveCost);
			terrainPiece = (GameObject) Instantiate(Logic.Inst.Terrains.GetHill(), transform.position, Quaternion.Euler(Vector3.right * 270f));
			break;

		case TerrainType.Mountains:
			moveCost = Mathf.Max(moveCost, TerrainModifiers.inst.mountains.moveCost);
			terrainPiece = (GameObject) Instantiate(Logic.Inst.Terrains.GetMountain(), transform.position, Quaternion.Euler(Vector3.right * 270f));
			isPassable = false;
			break;
		}

		if(terrainPiece){
			terrainPiece.transform.parent = transform;
			terrainPiece.GetComponentInChildren<MeshRenderer>().material = Logic.Inst.Terrains.GetBiomeMaterial(this.biome);
		}

		GetComponent<MeshRenderer>().material = Logic.Inst.Terrains.GetBiomeMaterial(this.biome);
	}

	public void ClearHerustics(){
		GCost = 0;
		HCost = 0;
		Parent = null;
		PathCost = int.MaxValue;
	}

	public static Tile operator+ (Tile one, Tile two){
		Tile ret = new Tile();
		ret.index = one.index + two.index;
		return ret;
	}

	public void LineColour(Color colour) {
		LineRenderer lines = GetComponent<LineRenderer>();
		if(lines)
			lines.SetColors(colour, colour);
	}

	public void LineColour(Color start, Color end){
		LineRenderer lines = GetComponent<LineRenderer>();
		if(lines)
			lines.SetColors(start, end);
	}

	public void LineWidth(float width){
		LineRenderer lines = GetComponent<LineRenderer>();
		if(lines)
			lines.SetWidth(width, width);
	}

	public void LineWidth(float start, float end){
		LineRenderer lines = GetComponent<LineRenderer>();
		if(lines)
			lines.SetWidth(start, end);
	}

	#region A* Herustic Variables
	public int MoveCost { 
		get {return moveCost;} 
		set {
			moveCost = value;
		}
	}
	public int PathCost { get; set;}
	public int GCost { get; set; }
	public int HCost { get; set; }
	public int FCost { get { return GCost + HCost; } }
	public Tile Parent { get; set; }

	public int CompareTo(Tile t){
		int c = FCost.CompareTo(t.FCost);

		if(c == 0)
			c = HCost.CompareTo(t.HCost);

		return -c;
	}
	#endregion

	#region Getters and Setters
	public CubeIndex Index {
		get { return index; }
		set { index = value; }
	}
	
	public Unit OccupyingUnit {
		get { return occupyingUnit; }
		set { occupyingUnit = value; }
	}

	public bool IsPassable {
		get { return isPassable; }
		set { isPassable = value; }
	}

	public BiomeType Biome{
		get {return biome;}
		set {biome = value;}
	}

	public TerrainType Terrain {
		get {return terrain;}
		set {terrain = value;}
	}

	public bool HasAltar {
		get {return hasAltar;}
		set {hasAltar = value;}
	}

	public int HeapIndex {get;set;}
	#endregion
}

[System.Serializable]
public struct OffsetIndex {
	public int row;
	public int col;

	public OffsetIndex(int row, int col){
		this.row = row; this.col = col;
	}
}

[System.Serializable]
public struct CubeIndex {
	public int x;
	public int y;
	public int z;

	public CubeIndex(int x, int y, int z){
		this.x = x; this.y = y; this.z = z;
	}

	public CubeIndex(int x, int z) {
		this.x = x; this.z = z; this.y = -x-z;
	}

	public static CubeIndex operator+ (CubeIndex one, CubeIndex two){
		return new CubeIndex(one.x + two.x, one.y + two.y, one.z + two.z);
	}

	public static bool operator== (CubeIndex lhs, CubeIndex rhs){
		return (lhs.x == rhs.x) && (lhs.y == rhs.y) && (lhs.z == rhs.z);
	}

	public static bool operator!= (CubeIndex lhs, CubeIndex rhs){
		return (lhs.x != rhs.x) || (lhs.y != rhs.y) || (lhs.z != rhs.z);
	}

	public override bool Equals (object obj) {
		if(obj == null)
			return false;
		CubeIndex o = (CubeIndex)obj;
		if((System.Object)o == null)
			return false;
		return((x == o.x) && (y == o.y) && (z == o.z));
	}

	public override int GetHashCode () {
		return(x.GetHashCode() ^ (y.GetHashCode() + (int)(Mathf.Pow(2, 32) / (1 + Mathf.Sqrt(5))/2) + (x.GetHashCode() << 6) + (x.GetHashCode() >> 2)));
	}

	public override string ToString () {
		return string.Format("[" + x + "," + y + "," + z + "]");
	}
}