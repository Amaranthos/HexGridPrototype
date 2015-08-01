using UnityEngine;

public class Tile : MonoBehaviour {

	private PairInt index;
	private Unit occupyingUnit = null;

	private void OnMouseUp() {
		Logic.Inst.TileClicked(this);
	}

	#region Getters and Setters
	public PairInt Index {
		get { return index; }
		set { index = value; }
	}

	public Unit OccupyngUnit {
		get { return occupyingUnit; }
		set { occupyingUnit = value; }
	} 
	#endregion
}
