using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
	private Queue<Tile> currentPath;

	private List<Tile> highlighted = new List<Tile>();

	private void Start() {
		currentHP = maxHitpoints;
		currentMP = movePoints;
	}

	private void OnMouseEnter() {
		Logic.Inst.InfoPanel.UpdateToolTip(this);
	}

	private void OnMouseExit() {
		Logic.Inst.InfoPanel.UpdateToolTip(null);
	}

	public void MoveTowardsTile(Tile tile) {
		Logic.Inst.Audio.PlaySFX(SFX.Unit_Move);

		Logic.Inst.Grid.TileAt(index).OccupyingUnit = null;

		if (Logic.Inst.gamePhase == GamePhase.CombatPhase){
			currentPath = new Queue<Tile>(Logic.Inst.Path.GetPath(Logic.Inst.Grid.TileAt(index), tile));
			currentPath = new Queue<Tile>(currentPath.Reverse());
			StartCoroutine(Move());
		}		
		else {
			transform.position = tile.transform.position;
		}

		index = tile.Index;
		tile.OccupyingUnit = this;
		//Handles adjacency buffs
		AdjacencyCheck();

		//Handles persistent passives
		PersistentAoECheck();

		Altar altar = Logic.Inst.GetAltar(tile.Index);

		if (altar)
			altar.PlayerCaptureAltar(owner);
		ClearHighlightedTiles();
	}

	private IEnumerator Move(){
		while(currentPath.Count != 0){
			Tile tile = currentPath.Dequeue();
			// StartCoroutine(Rotate(tile, 0.1f));
			transform.position = tile.transform.position;
			currentMP -= tile.MoveCost;
			yield return new WaitForSeconds(0.5f);
		}
		yield return null;
	}

	private IEnumerator Rotate(Tile target, float step) {
		Quaternion dir = Quaternion.LookRotation(target.transform.position - transform.position);
		while(transform.rotation != dir){
			transform.rotation = Quaternion.RotateTowards(transform.rotation, dir, 0.1f);
			yield return new WaitForSeconds(0.1f);
		}
		yield return null;		
	}

	public void UnitSelected(){
		Debug.Log(type + " selected");
		List<Tile> ret = Logic.Inst.Grid.TilesInRange(index, currentMP);

		foreach(Tile tile in ret){
			if(tile.OccupyingUnit != null){
				if(tile.OccupyingUnit.Owner != Owner && canMove)
					if(Logic.Inst.Grid.Distance(tile, Logic.Inst.Grid.TileAt(index)) <= attackRange){
						tile.LineColour(Color.red);
						tile.LineWidth(0.1f);
						highlighted.Add(tile);
					}
				else if(InMoveRange(tile)){
					tile.LineColour(Color.grey);
					tile.LineWidth(0.1f);
					highlighted.Add(tile);
				}
			}
			else if(InMoveRange(tile)){
				tile.LineColour(Color.green);
				tile.LineWidth(0.1f);
				highlighted.Add(tile);
			}
		}

		Logic.Inst.Grid.TileAt(index).LineColour(Color.cyan);
		Logic.Inst.Grid.TileAt(index).LineWidth(0.1f);
		highlighted.Add(Logic.Inst.Grid.TileAt(index));
	}

	public void ClearHighlightedTiles() {
		foreach(Tile tile in highlighted){
			tile.LineColour(Color.black);
			tile.LineWidth(0.03f);			
		}

		highlighted.Clear();
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
		return Logic.Inst.Grid.Neighbours(index).Contains(Logic.Inst.Grid.TileAt(unit.Index));
	}

	public bool InMoveRange(Tile tile) {
		List<Tile> ret = Logic.Inst.Grid.TilesInRange(index, currentMP);

		if(currentMP > 0)
			ret.RemoveAll(item => {
				int pathCost = Logic.Inst.Path.PathCost(Logic.Inst.Path.GetPath(Logic.Inst.Grid.TileAt(index), item));
				return (pathCost > currentMP || pathCost <= 0);
			});
		else
			ret.Clear();
		return ret.Contains(tile);
	}

	public void OnTurnStart(){
		currentMP = movePoints;

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
		bool newBuff = true;

		foreach(Buff buff in currentBuffs){
			if(buff.ID == bff.ID){
				newBuff = false;
				buff.duration = bff.duration;
			}
		}

		if(!bff.oneShot && newBuff){
			nEft = new Buff(bff.ID,bff.buffType,bff.duration,bff.effectType,bff.strength,bff.wrath,bff.targetType,bff.permanent,bff.oneShot,bff.adjType,bff.adjUnits,bff.isBio,bff.terType,bff.bioType);
			currentBuffs.Add(nEft);
		}

		if(bff.buffType == BuffType.Stat && newBuff){
			bff.ChangeValue(this,true);
		}
	}

	public void AdjacencyCheck(){
		foreach(Buff buff in currentBuffs){
			List<Tile> inRange = new List<Tile>();
			inRange = Logic.Inst.Grid.TilesInRange(index,1);
			
			if(buff.buffType == BuffType.Adjacent){
				CalculateModifiers();
			}
			
			foreach(Tile adjTile in inRange){
				if(adjTile.OccupyingUnit){
					foreach(Buff adjBuff in adjTile.OccupyingUnit.currentBuffs){
						if(adjBuff.buffType == BuffType.Adjacent){
							adjTile.OccupyingUnit.CalculateModifiers();
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
			if((bff.duration > 0 || bff.permanent) && bff.buffType == BuffType.Stat){
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
					if(tile.OccupyingUnit){
						switch(buff.adjType){
						case AdjacencyType.Friends:
							if(buff.adjUnits.Contains(tile.OccupyingUnit.type) && tile.OccupyingUnit.owner == owner){
								buff.ChangeValue(this,true);
								proced = true;
							}
							break;

						case AdjacencyType.Enemies:
							if(buff.adjUnits.Contains(tile.OccupyingUnit.type) && tile.OccupyingUnit.owner != owner){
								buff.ChangeValue(this,true);
								proced = true;
							}
							break;

						case AdjacencyType.Both:
							if(buff.adjUnits.Contains(tile.OccupyingUnit.type)){
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
		int buffToRemove = -1;
		inRange = Logic.Inst.Grid.TilesInRange(index,owner.hero.passive.AoERange);

		if(type == UnitType.Hero && owner.hero.passive.passive == PassiveType.PersitentAoE){
			foreach(Unit unit in owner.army){
				foreach(Buff buff in unit.currentBuffs){
					foreach(Buff passBuff in owner.hero.passive.buffs){
						if(buff.ID == passBuff.ID){
							//unit.currentBuffs.Remove(buff);	//This is probably going to break. It did.
							buffToRemove = unit.currentBuffs.IndexOf(buff);
						}
					}
				}

				if(buffToRemove > -1){
					unit.currentBuffs[buffToRemove].ChangeValue(unit,false);
					unit.currentBuffs.RemoveAt(buffToRemove);
					buffToRemove = -1;
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
//							currentBuffs.Remove(buff);	//This is probably going to break.
							buffToRemove = currentBuffs.IndexOf(buff);
						}
					}
				}
				if(buffToRemove > -1){
					currentBuffs[buffToRemove].ChangeValue(this,false);
					currentBuffs.RemoveAt(buffToRemove);
					buffToRemove = -1;
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

	public int CurrentMovePoints {
		get { return currentMP; }
		set { currentMP = value; }
	}
	#endregion
}