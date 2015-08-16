using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

	public Text turnInfo;
	public Text unitInfo;
	public Text unitHInfo;
	public Text toolTipInfo;
	public Text tileInfo;

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
			turnInfo.text = player.name + "'s turn" + "\n" + Logic.Inst.gamePhase.ToString();
	}

	public void UpdateToolTip(Unit unit)
	{
		if (unit != null)
			toolTipInfo.text = unit.type + "\nHp: " + unit.CurrentHitpoints + "/" + unit.maxHitpoints + "\nAtk: " + unit.attack + "\nDef: " + unit.defense + "\nHit %: " + unit.hitChance + "\nDodge %: " + unit.dodgeChance + "\nRange: " + unit.attackRange + "\nMove Pts: " + unit.movePoints;
		else
			toolTipInfo.text = "";
	}
	
	public void UpdateTileInfo(Tile tile) {
		if (!showPanel)
			return;

		if (tile)
			tileInfo.text = "Selected tile: " + tile.Index.x + "," + tile.Index.y + "\nPassable: " + tile.IsPassable + "\nOccupied: " + (tile.OccupyngUnit != null).ToString();
		else
			tileInfo.text = "Selected tile: \nPassable: \nOccupied: ";
	}

	public void UpdateUnitInfo(Unit unit) {
		if (!showPanel)
			return;

		if (unit)
			unitInfo.text = "Selected Unit: " + unit.type + "\nHp: " + unit.CurrentHitpoints + "/" + unit.maxHitpoints + "\nAtk: " + unit.attack + "\nDef: " + unit.defense + "\nHit %: " + unit.hitChance + "\nDodge %: " + unit.dodgeChance + "\nRange: " + unit.attackRange + "\nMove Pts: " + unit.movePoints;
		else
			unitInfo.text = "Selected Unit: \nHp: \nAtk: \nDef: \nHit %: \nDodge %: \nRange: \nMove Pts: ";
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