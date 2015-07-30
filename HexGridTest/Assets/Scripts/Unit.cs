using UnityEngine;

public class Unit : MonoBehaviour {

	public UnitType type;
	private HG.PairInt index;
	public bool isSelected = false;

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
		if (owner == Logic.inst.CurrentPlayer) {
			Logic.inst.UnitSelected(this);
			isSelected = true;
		}
	}

	public void MoveTowardsTile(Tile tile) {
		transform.position = tile.transform.position;
		index = tile.Index;
		tile.IsOccupied = true;
		tile.OccupyngUnit = this;
	}

	public void UnitKilled() {
		
	}

	#region Getters and Setters
	public HG.PairInt Index {
		get { return index; }
		set { index = value; }
	}

	public bool IsSelected {
		get { return isSelected; }
		set { isSelected = value; }
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
