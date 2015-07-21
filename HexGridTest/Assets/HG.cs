using UnityEngine;
using System.Collections;

namespace HG {
	[System.Serializable]
	public struct PairInt {
		public int x;
		public int y;

		PairInt(int x, int y) {
			this.x = x;
			this.y = y;
		}
	}
}
