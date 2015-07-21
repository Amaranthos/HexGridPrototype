using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	public GameObject hexObj;

	public HG.PairInt gridSize;

	private void Start() {
		GenerateGrid();
	}

	private void GenerateGrid() {

		int numAlongSide = 4;

		for (int q = -gridSize.x / 2; q <= gridSize.x / 2; q++) {
			int r1 = Mathf.Max(-gridSize.x / 2, -q - gridSize.x / 2);
			int r2 = Mathf.Min(gridSize.x/2, -q + gridSize.x/2);

			for (int r = r1; r <= r2; r++) {
				Instantiate(hexObj, new Vector3(q, 0, r), Quaternion.identity);
			}
		}

		//for (int i = 0; i < gridSize.x; i++) {
		//	for (int j = 0; j < gridSize.y; j++) {
		//		Instantiate(hexObj, new Vector3(i, 0.0f, j + (i % 2 * 0.5f)), Quaternion.Euler(Vector3.up * 90));
		//	}
		//}
	}
}
