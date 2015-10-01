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
	private int currentMP;

	private bool canMove = true;

	private Player owner;

	public List<Buff> currentBuffs = new List<Buff>();	//Every effect this unit is currently under

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
//		if (Logic.Inst.gamePhase == GamePhase.CombatPhase) {
//			List<Tile> path = Logic.Inst.Path.GetPath(Logic.Inst.Grid.GetTile(index), tile);
//
//			foreach (Tile t in path)
//				t.SetLineColour(Color.cyan);
//		}
		Logic.Inst.Audio.PlaySFX(SFX.Unit_Move);
		Logic.Inst.Grid.GetTile(index).OccupyngUnit = null;
		index = tile.Index;
		tile.OccupyngUnit = this;

		transform.position = tile.transform.position;

		if(owner.hero.type == HeroType.Heimdal){
			foreach(Unit unit in owner.army){
				CalculateModifiers();
			}
		}

		Altar altar = Logic.Inst.GetAltar(tile.Index);

		if (altar)
			altar.PlayerCaptureAltar(owner);
	}

	public void UnitKilled() {
		owner.RemoveUnit(this);
		Debug.Log(type + " was killed");
		DestroyImmediate(this.gameObject);
		Logic.Inst.Audio.PlaySFX(SFX.Unit_Death);
	}

	public void UnitSacrificed() {
		owner.Faith += Logic.Inst.faithPtsPerSacrifice;
		owner.RemoveUnit(this);
		Debug.Log(type + " was sacrificed");
		DestroyImmediate(this.gameObject);
		Logic.Inst.Audio.PlaySFX(SFX.Unit_Death);
	}

	public bool InAttackRange(Unit unit) {
		return Logic.Inst.Grid.GetNeighbours(index.x, index.y).Contains(Logic.Inst.Grid.GetTile(unit.Index.x, unit.Index.y));
	}

	public bool InMoveRange(Tile tile) {
		return Logic.Inst.Grid.TilesInRange(index, movePoints).Contains(tile);
	}

	public void OnTurnStart(){
		foreach (Buff eft in currentBuffs){
			eft.duration--;
			if(eft.duration == 0){				//If the effect is done
				eft.ChangeValue(this,false);	//Alter this units relative stat. False indicates that the effect is being removed.
//				currentBuffs.Remove(eft);		//Removing the effect at this point will mess with the list and break things. For now, the effects will be permanent but the stats will still be removed correctly.
//				if(eft.skadiWrath){				//Oh god I need a better check than this.
//					owner.hero.skadiWrathCheck = false;
//				}
			}
		}

		CalculateModifiers();

		if(type == UnitType.Hero){
//			gameObject.GetComponent<Hero>().ApplyPassive();
		}
	}

	public void AddBuff(Buff bff){
		Buff nEft;

		if(!bff.oneShot){
			nEft = new Buff(bff.ID,bff.buffType,bff.duration,bff.effectType,bff.strength,bff.wrath,bff.targetType,bff.oneShot,bff.adjType,bff.adjUnits,bff.terType);
			currentBuffs.Add(nEft);
		}

		if(bff.buffType == BuffType.Stat){
			bff.ChangeValue(this,true);
		}
	}

	public void CalculateModifiers(){
		attackModifer = 0;
		defenseModifer = 0;
		hitModifier = 0;
		dodgeModifier = 0;
		
		CheckForAllies();
		foreach (Buff eft in currentBuffs){
			if(eft.duration > 0){
				eft.ChangeValue(this,true);
			}
		}
	}

	public void CheckForAllies(){
		List<Tile> inRange = new List<Tile>();

		if(owner.hero.type == HeroType.Heimdal){
			inRange = Logic.Inst.Grid.AbilityRange(index,1);
			foreach(Tile tile in inRange){
				if(tile.OccupyngUnit){
					if(tile.OccupyngUnit.owner == owner){
//						defenseModifer += owner.hero.passive.effects[0].strength;
					}
				}
			}
//			defenseModifer -= owner.hero.passive.effects[0].strength;	//To account for the fact that the unit will count itself.
		}
//		else if(owner.hero.type == HeroType.Skadi){			//Super Broken. Not friendly right now.
//			if(owner.hero.skadiWrathCheck){		//Need a better check here to see if her Active 2 wrath effect has been applied.
//				inRange = Logic.Inst.Grid.AbilityRange(index,1);
//				foreach(Tile tile in inRange){
//					if(tile.OccupyngUnit){
//						if(tile.OccupyngUnit.owner == owner && tile.OccupyngUnit.type == UnitType.Spearman && type == UnitType.Spearman){
//							attackModifer += owner.hero.active2.effects[2].strength;
//						}
//					}
//				}
//
//				attackModifer -= owner.hero.active2.effects[2].strength * 2; //To account for the unit counting itself, and for the initial buff effects.
//			}
//		}
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

	public bool CanMove {
		get { return canMove; }
		set { canMove = value; }
	}

	public int CurrentMocePoints {
		get { return currentMP; }
		set { currentMP = value; }
	}
	#endregion
}