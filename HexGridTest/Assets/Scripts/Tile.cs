using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	private HG.PairInt index;
	public bool isSelected = false;
	public bool isOccupied = false;


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
}
