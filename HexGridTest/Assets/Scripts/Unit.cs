using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	public UnitType type;
	private HG.PairInt index;
	public bool isSelected = false;

	[Range(0, 5)]
	public int movePoints;

	private void OnMouseUp() {
		Logic.inst.UnitSelected(this);
		isSelected = true;
	}

	public void MoveTowardsTile(Tile tile) {
		transform.position = tile.transform.position;
		index = tile.Index;
		tile.isOccupied = true;
	}

	public HG.PairInt Index {
		get { return index; }
		set { index = value; }
	}

	public bool IsSelected {
		get { return isSelected; }
		set { isSelected = value; }
	}
}
