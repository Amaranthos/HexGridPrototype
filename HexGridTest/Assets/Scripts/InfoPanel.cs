using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

	public Text turnInfo;
	public Text unitInfo;
	public Text unitHInfo;
	public Text tileInfo;

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
			turnInfo.text = player.name + "'s turn" + "\nFood Remaining: " + player.CurrentFood + "\nCommand Points: " + player.CurrentCommandPoints + "/" + player.commandPoints;
	}

	public void UpdateTileInfo(Tile tile) {
		if (!showPanel)
			return;

		if (tile)
			tileInfo.text = "Selected tile: " + tile.Index.x + "," + tile.Index.y;
		else
			tileInfo.text = "Selected tile: ";
	}

	public void UpdateUnitInfo(Unit unit) {
		if (!showPanel)
			return;

		if (unit)
			unitInfo.text = "Selected Unit: " + unit.type + "\nHp: " + unit.CurrentHitpoints + "/" + unit.maxHitpoints + "\nAttack: " + unit.attack + "\nDefense: " + unit.defense + "\nHit Chance: " + unit.hitChance + "\nDodge Chance: " + unit.dodgeChance + "\nRange: " + unit.attackRange + "\nMove Points: " + unit.movePoints;
		else
			unitInfo.text = "Selected Unit: \n Hp: \n Attack: \n Defense: \n Hit Chance: \n Dodge Chance: \n Range: \n Move Points: ";
	}

	public void UpdateUnitHInfo(Unit unit) {
		if (!showPanel)
			return;

		if (unit)
			unitHInfo.text = "Hovered Unit: " + unit.type + "\nHp: " + unit.CurrentHitpoints + "/" + unit.maxHitpoints + "\nAttack: " + unit.attack + "\nDefense: " + unit.defense + "\nHit Chance: " + unit.hitChance + "\nDodge Chance: " + unit.dodgeChance + "\nRange: " + unit.attackRange + "\nMove Points: " + unit.movePoints;
		else
			unitHInfo.text = "Hovered Unit: \n Hp: \n Attack: \n Defense: \n Hit Chance: \n Dodge Chance: \n Range: \n Move Points: ";
	}
}