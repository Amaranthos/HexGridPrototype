using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

	public Text turnInfo;
	public Text unitInfo;
	public Text unitHInfo;
	public Text tileInfo;

	public GameObject canvas;

	public bool showPanel = true;

	public void Clear() {
		UpdateTurnInfo(null);
		UpdateTileInfo(null);
		UpdateUnitInfo(null);
		UpdateUnitHInfo(null);
	}

	public void UpdateTurnInfo(Player player) {
		if (!showPanel)
			return;
		if(player)
			turnInfo.text = player.name + "'s turn" + "\nFood Remaining: " + player.CurrentFood;
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
			unitInfo.text = "Selected Unit: " + unit.type + "\nHp: " + unit.CurrentHitpoints + "/" + unit.maxHitpoints + "\nAttack: " + unit.attack + "\nDefense: " + unit.defense + "\nHit Chance: " + unit.hitChance + "\nDodge Chance: " + unit.dodgeChance + "\nRange: " + unit.attackRange + "\nMove Points: " + unit.movePoints;
		else
			unitInfo.text = "Selected Unit: \nHp: \nAttack: \nDefense: \nHit Chance: \nDodge Chance: \nRange: \nMove Points: ";
	}

	public void UpdateUnitHInfo(Unit unit) {
		if (!showPanel)
			return;

		if (unit)
			unitHInfo.text = "Hovered Unit: " + unit.type + "\nHp: " + unit.CurrentHitpoints + "/" + unit.maxHitpoints + "\nAttack: " + unit.attack + "\nDefense: " + unit.defense + "\nHit Chance: " + unit.hitChance + "\nDodge Chance: " + unit.dodgeChance + "\nRange: " + unit.attackRange + "\nMove Points: " + unit.movePoints;
		else
			unitHInfo.text = "Hovered Unit: \nHp: \nAttack: \nDefense: \nHit Chance: \nDodge Chance: \nRange: \nMove Points: ";
	}

	public void Enabled(bool enabled) {
		canvas.SetActive(enabled);
	}
}