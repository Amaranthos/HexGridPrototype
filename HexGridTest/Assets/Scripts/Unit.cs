using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	public UnitType type;
	private PairInt index;

	[Range(0, 5)]
	public int movePoints;
	public int maxHitpoints;
	public int attack;
	public int defense;

	[Range(0,100)]
	public int hitChance;
	[Range(0,100)]
	public int dodgeChance;
	public int attackRange;
	private int attackModifer = 0;
	private int defenseModifer = 0;
	private int hitModifier = 0;
	private int dodgeModifier = 0;
	[SerializeField]
	private int currentHP;

	[SerializeField]
	private Player owner;

	private void Start() {
		currentHP = maxHitpoints;
	}

	private void OnMouseUp() {
		Logic.Inst.UnitClicked(this);
	}

	private void OnMouseEnter() {
		Logic.Inst.InfoPanel.UpdateUnitHInfo(this);
	}

	private void OnMouseExit() {
		Logic.Inst.InfoPanel.UpdateUnitHInfo(null);
	}

	public void MoveTowardsTile(Tile tile) {
		transform.position = tile.transform.position;
		index = tile.Index;
		tile.OccupyngUnit = this;
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
	#endregion
}
