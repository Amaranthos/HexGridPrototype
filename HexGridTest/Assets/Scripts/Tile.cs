using UnityEngine;

public class Tile : MonoBehaviour {

	private HG.PairInt index;
	private bool isSelected = false;
	private bool isOccupied = false;

	public Unit occupyingUnit = null;

	private void OnMouseUp() {
		Logic.inst.TileSelected(this);
		isSelected = true;
	}

	public HG.PairInt Index {
		get { return index; }
		set { index = value; }
	}

	public bool IsSelected {
		get { return isSelected; }
		set { isSelected = value; }
	}

	public bool IsOccupied {
		get {return isOccupied;}
		set {isOccupied = value;}
	}

	public Unit OccupyngUnit {
		get { return occupyingUnit; }
		set { occupyingUnit = value; }
	} 
}
