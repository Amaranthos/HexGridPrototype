using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	public GameObject hexObj;

	public HG.PairInt gridSize;

	private void Start() {
		GenerateGrid();
	}

	private void GenerateGrid() {

		for (int q = -gridSize.x; q <= gridSize.x; q++) {
			int r1 = Mathf.Max(-gridSize.x, -q - gridSize.x);
			int r2 = Mathf.Min(gridSize.x, -q + gridSize.x);

			for (int r = r1; r <= r2; r++) {
				Instantiate(hexObj, Hex.CubeCoords(q, r), Quaternion.identity);//Quaternion.Euler(Vector3.up * 90));
			}
		}

		//for (int i = 0; i < gridSize.x; i++) {
		//	for (int j = 0; j < gridSize.y; j++) {
		//		Instantiate(hexObj, new Vector3(i, 0.0f, j + (i % 2 * 0.5f)), Quaternion.Euler(Vector3.up * 90));
		//	}
		//}
	}
}
