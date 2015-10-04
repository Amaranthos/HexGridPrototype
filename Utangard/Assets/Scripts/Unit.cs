using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	public UnitType type;
	private CubeIndex index;

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
		Logic.Inst.Grid.TileAt(index).OccupyngUnit = null;
		index = tile.Index;
		tile.OccupyngUnit = this;

		transform.position = tile.transform.position;

		//Handles adjacency buffs
		AdjacencyCheck();

		//Handles persistent passives
		PersistentAoECheck();


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
		return Logic.Inst.Grid.Neighbours(index.x, index.y).Contains(Logic.Inst.Grid.TileAt(unit.Index.x, unit.Index.y));
	}

	public bool InMoveRange(Tile tile) {
		return Logic.Inst.Grid.TilesInRange(index, movePoints).Contains(tile);
	}

	public void OnTurnStart(){
		foreach (Buff bff in currentBuffs){
			bff.duration--;
			if(bff.duration == 0){				//If the effect is done
				bff.ChangeValue(this,false);	//Alter this units relative stat. False indicates that the effect is being removed.
//				currentBuffs.Remove(eft);		//Removing the effect at this point will mess with the list and break things. For now, the effects will be permanent but the stats will still be removed correctly.
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
			nEft = new Buff(bff.ID,bff.buffType,bff.duration,bff.effectType,bff.strength,bff.wrath,bff.targetType,bff.permanent,bff.oneShot,bff.adjType,bff.adjUnits,bff.terType);
			currentBuffs.Add(nEft);
		}

		if(bff.buffType == BuffType.Stat){
			bff.ChangeValue(this,true);
		}
	}

	public void AdjacencyCheck(){
		foreach(Buff buff in currentBuffs){
			List<Tile> inRange = new List<Tile>();
//			inRange = Logic.Inst.Grid.AbilityRange(index,1);
			inRange = Logic.Inst.Grid.TilesInRange(index,1);
			
			if(buff.buffType == BuffType.Adjacent){
				CalculateModifiers();
			}
			
			foreach(Tile adjTile in inRange){
				if(adjTile.OccupyngUnit){
					foreach(Buff adjBuff in adjTile.OccupyngUnit.currentBuffs){
						if(adjBuff.buffType == BuffType.Adjacent){
							adjTile.OccupyngUnit.CalculateModifiers();
						}
					}
				}
			}
		}
	}

	public void CalculateModifiers(){
		attackModifer = 0;
		defenseModifer = 0;
		hitModifier = 0;
		dodgeModifier = 0;
		
		CalcAdjacency();

		foreach (Buff bff in currentBuffs){
			if((bff.duration > 0 || bff.permanent) && bff.buffType != BuffType.Adjacent){
				bff.ChangeValue(this,true);
			}
		}
	}

	public void CalcAdjacency(){
		List<Tile> inRange = new List<Tile>();
		bool proced = false;

		foreach(Buff buff in currentBuffs){
			proced = false;
			if(buff.buffType == BuffType.Adjacent){
//				inRange = Logic.Inst.Grid.AbilityRange(index,1);
				inRange = Logic.Inst.Grid.TilesInRange(index,1);
				foreach(Tile tile in inRange){
					if(tile.OccupyngUnit){
						switch(buff.adjType){
						case AdjacencyType.Friends:
							if(buff.adjUnits.Contains(tile.OccupyngUnit.type) && tile.OccupyngUnit.owner == owner){
								buff.ChangeValue(this,true);
								proced = true;
							}
							break;

						case AdjacencyType.Enemies:
							if(buff.adjUnits.Contains(tile.OccupyngUnit.type) && tile.OccupyngUnit.owner != owner){
								buff.ChangeValue(this,true);
								proced = true;
							}
							break;

						case AdjacencyType.Both:
							if(buff.adjUnits.Contains(tile.OccupyngUnit.type)){
								buff.ChangeValue(this,true);
								proced = true;
							}
							break;

						default:
							break;

						}
					}
				}

				if(proced){
					buff.ChangeValue(this,false); //To account for the fact that the unit will count itself.
				}
			}
		}
	}

	public void PersistentAoECheck(){
		List<Tile> inRange = new List<Tile>();
//		inRange = Logic.Inst.Grid.AbilityRange(index,owner.hero.passive.AoERange);
		inRange = Logic.Inst.Grid.TilesInRange(index,owner.hero.passive.AoERange);

		if(type == UnitType.Hero && owner.hero.passive.passive == PassiveType.PersitentAoE){
			foreach(Unit unit in owner.army){
				foreach(Buff buff in unit.currentBuffs){
					foreach(Buff passBuff in owner.hero.passive.buffs){
						if(buff.ID == passBuff.ID){
							unit.currentBuffs.Remove(buff);	//This is probably going to break.
						}
					}
				}
			}
			
			owner.hero.passive.ApplyBuffAoE(index);
		}
		else if(owner.hero.passive.passive == PassiveType.PersitentAoE){
			if(inRange.Contains(Logic.Inst.Grid.TileAt(owner.hero.gameObject.GetComponent<Unit>().index))){
				owner.hero.passive.ApplyBuffSingle(index);
			}
			else{
				foreach(Buff buff in currentBuffs){
					foreach(Buff passBuff in owner.hero.passive.buffs){
						if(buff.ID == passBuff.ID){
							currentBuffs.Remove(buff);	//This is probably going to break.
						}
					}
				}
			}
		}
	}
		
	#region Getters and Setters
	public CubeIndex Index {
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