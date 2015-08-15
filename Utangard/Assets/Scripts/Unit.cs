using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	public UnitType type;
	private PairInt index;

	public int movePoints;
	public int maxHitpoints;
	public int attack;
	public int defense;
	public int cost;

	[Range(0,100)]
	public int hitChance;
	[Range(0,100)]
	public int dodgeChance;
	public int attackRange;
	private int attackModifer = 0;
	private int defenseModifer = 0;
	private int hitModifier = 0;
	private int dodgeModifier = 0;
	private int currentHP;

	private bool hasMoved = false;

	private Player owner;

	private void Start() {
		currentHP = maxHitpoints;
	}

	private void OnMouseEnter() {
		Logic.Inst.InfoPanel.UpdateToolTip(this);
	}

	private void OnMouseExit() {
		Logic.Inst.InfoPanel.UpdateToolTip(null);
	}

	public void MoveTowardsTile(Tile tile) {
		Logic.Inst.Grid.GetTile(index).OccupyngUnit = null;
		index = tile.Index;
		tile.OccupyngUnit = this;

		transform.position = tile.transform.position;

		if (Logic.Inst.gamePhase == GamePhase.CombatPhase)
			hasMoved = true;
	}

	public void UnitKilled() {
		owner.RemoveUnit(this);
		Debug.Log(type + " was killed");
		DestroyImmediate(this.gameObject);
	}

	public bool InAttackRange(Unit unit) {
		return Logic.Inst.Grid.GetNeighbours(index.x, index.y).Contains(Logic.Inst.Grid.GetTile(unit.Index.x, unit.Index.y));
	}

	public bool InMoveRange(Tile tile) {
		return Logic.Inst.Grid.GetNeighbours(index.x, index.y).Contains(tile);
	}
	
	#region Getters and Setters
	public PairInt Index {
		get { return index; }
		set { index = value; }
	}

	public int CurrentHitpoints {
		get { return currentHP; }
		set { currentHP = value; }
	}

	public int AttackModifier {
		get { return attackModifer; }
		set { attackModifer = value; }
	}

	public int DefenseModifier {
		get { return defenseModifer; }
		set { defenseModifer= value; }
	}

	public int HitModifier {
		get { return hitModifier; }
		set { hitModifier = value; }
	}

	public int DodgeModifier {
		get { return dodgeModifier; }
		set { dodgeModifier = value; }
	}

	public int TotalAttack {
		get { return attack + attackModifer; }
	}

	public int TotalDefense {
		get { return defense + defenseModifer; }
	}

	public int TotalHitChance {
		get { return hitChance + hitModifier; }
	}

	public int TotalDodgeChance {
		get { return dodgeChance + dodgeModifier; }
	}

	public Player Owner {
		get { return owner; }
		set { owner = value; }
	}

	public bool HasMoved {
		get { return hasMoved; }
		set { hasMoved = value; }
	}
	#endregion
}
