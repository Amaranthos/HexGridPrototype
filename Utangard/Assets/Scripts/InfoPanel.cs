using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

	[System.Serializable]
	public class TextFields
	{
		public Text turnInfo;
		public Text unitInfoP1;
		public Text unitInfoP2;
		public Text unitHInfo;
		public Text toolTipInfo;
		public Text tileInfoP1;
		public Text tileInfoP2;
	}
	public TextFields text;
	public GameObject canvas;

	public bool showPanel = true;

	public void Clear() {
		UpdateTurnInfo(null);
		UpdateTileInfo(null);
		UpdateUnitInfo(null);
		//UpdateUnitHInfo(null);
	}

	public void UpdateTurnInfo(Player player) {
		if (!showPanel)
			return;
		if(player)
			text.turnInfo.text = player.name + "'s turn" + "\n" + Logic.Inst.gamePhase.ToString();
	}

	public void UpdateToolTip(Unit unit)
	{
		if (unit != null)
			text.toolTipInfo.text = (unit.type
				+ "\n<size=15>"
				+ " Pos: " + Logic.Inst.Grid.TileAt(unit.Index).ToString()
				+ "</size>\n<size=14>"
				+ " Hp: " + unit.CurrentHitpoints + "/" + unit.maxHitpoints
				+ "\n Move Pts: " + unit.movePoints 
				+ "\n Atk: " + unit.attack
				+ "\n Def: " + unit.defense 
				+ "\n Hit %: " + unit.hitChance 
				+ "\n Dodge %: " + unit.dodgeChance 
				+ "\n Range: " + unit.attackRange
				+ "</size>");

		else
			text.toolTipInfo.text = "";
	}
	
	public void UpdateTileInfo(Tile tile) {
		if (!showPanel)
			return;

		if (tile){
			text.tileInfoP1.text = "Selected tile: " + tile.Index.x + "," + tile.Index.y + "\nPassable: " + tile.IsPassable + "\nOccupied: " + (tile.OccupyingUnit != null).ToString();
			text.tileInfoP2.text = "Selected tile: " + tile.Index.x + "," + tile.Index.y + "\nPassable: " + tile.IsPassable + "\nOccupied: " + (tile.OccupyingUnit != null).ToString();
		}
		else{
			text.tileInfoP1.text = "Selected tile: \nPassable: \nOccupied: ";
			text.tileInfoP2.text = "Selected tile: \nPassable: \nOccupied: ";
		}
	}

	public void UpdateUnitInfo(Unit unit) {
		if (!showPanel)
			return;

		if (unit){
			// if p1 is current player
			if(Logic.Inst.CurrentPlayerNum == 0)
				text.unitInfoP1.text = ("Selected Unit: " + unit.type 
				                        + "\n Pos: " + Logic.Inst.Grid.TileAt(unit.Index).ToString()
				                        + "\n Hp: " + unit.CurrentHitpoints + "/" + unit.maxHitpoints 
				                        + "\n Atk: " + unit.attack 
				                        + "\n Def: " + unit.defense 
				                        + "\n Hit %: " + unit.hitChance 
				                        + "\n Dodge %: " + unit.dodgeChance 
				                        + "\n Range: " + unit.attackRange 
				                        + "\n Move Pts: " + unit.movePoints);
			else
				text.unitInfoP2.text = ("Selected Unit: " + unit.type 
				                        + "\n Pos: " + Logic.Inst.Grid.TileAt(unit.Index).ToString()
				                        + "\n Hp: " + unit.CurrentHitpoints + "/" + unit.maxHitpoints 
				                        + "\n Atk: " + unit.attack 
				                        + "\n Def: " + unit.defense 
				                        + "\n Hit %: " + unit.hitChance 
				                        + "\n Dodge %: " + unit.dodgeChance 
				                        + "\n Range: " + unit.attackRange 
				                        + "\n Move Pts: " + unit.movePoints);
		}
		else{
			text.unitInfoP1.text = "Selected Unit: \nHp: \nAtk: \nDef: \nHit %: \nDodge %: \nRange: \nMove Pts: ";
			text.unitInfoP2.text = "Selected Unit: \nHp: \nAtk: \nDef: \nHit %: \nDodge %: \nRange: \nMove Pts: ";
		}
	}

	//public void UpdateUnitHInfo(Unit unit) {
	//	if (!showPanel)
	//		return;

	//	if (unit)
	//		unitHInfo.text = "Hovered Unit: " + unit.type + "\nHp: " + unit.CurrentHitpoints + "/" + unit.maxHitpoints + "\nAtk: " + unit.attack + "\nDef: " + unit.defense + "\nHit %: " + unit.hitChance + "\nDodge %: " + unit.dodgeChance + "\nRange: " + unit.attackRange + "\nMove Pts: " + unit.movePoints;
	//	else
	//		unitHInfo.text = "Hovered Unit: \nHp: \nAtk: \nDef: \nHit %: \nDodge %: \nRange: \nMove Pts: ";
	//}

	public void Enabled(bool enabled) {
		canvas.SetActive(enabled);
	}
}