using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Skill{
	public AbilityType abilityType;
	public AimType target;	//When in wrath mode, some abilities have an added AoE effect. May need to apply AoE to effects, rather than to abilites.
	public PassiveType passive;
	public bool hitFoe;
	public int cost;
	public int castRange;
	public int AoERange;
	public List<Target> targets = new List<Target>();
	public Unit hero;
	public List<UnitType> affected = new List<UnitType>();
	public List<Buff> buffs = new List<Buff>();
	
	public Skill(AbilityType at, AimType tt, bool hf, int cst, int crng, int arng, List<Target> targs, PassiveType pass, Unit hro, List<UnitType> afd, List<Buff> buf){
		abilityType = at;
		target = tt;
		hitFoe = hf;
		cost = cst;
		castRange = crng;
		AoERange = arng;
		targets = targs;
		passive = pass;
		hero = hro;
		affected = afd;
		buffs = buf;
	}

	public void ApplyBuffSingle(CubeIndex index){
		Unit unit = Logic.Inst.Grid.TileAt(index).OccupyingUnit;

		if(!hitFoe && unit.Owner == hero.Owner && affected.Contains(unit.type)){
			AddBuffs(unit);
		}
		else if (hitFoe && unit.Owner != hero.Owner && affected.Contains(unit.type)){
			AddBuffs(unit);
		}
	}

	public void ApplyBuffAll(int playerNo){
		foreach(Unit unit in Logic.Inst.Players[playerNo].army){
			if(affected.Contains(unit.type))	//Check if the unit type is affected by this ability
				AddBuffs(unit);
		}
	}

	public void ApplyBuffAoE(CubeIndex tileIndex){
		Debug.Log ("Called");
		List<Tile> inRange = new List<Tile>();

//		inRange = Logic.Inst.Grid.AbilityRange(tileIndex,AoERange);
		inRange = Logic.Inst.Grid.TilesInRange(tileIndex,AoERange);
		foreach(Tile tile in inRange){
			if(!hitFoe){
				if(tile.OccupyingUnit && tile.OccupyingUnit.Owner == hero.Owner && affected.Contains(tile.OccupyingUnit.type)){
					AddBuffs(tile.OccupyingUnit);
					Debug.Log("Adding Buff");
				}
			}
			else{
				if(tile.OccupyingUnit && tile.OccupyingUnit.Owner != hero.Owner && affected.Contains(tile.OccupyingUnit.type)){
					AddBuffs(tile.OccupyingUnit);
				}
			}
		}
	}
	
	public void AddBuffs(Unit unit){
		foreach(Buff buff in buffs){
			if(buff.wrath && hero.Owner.wrathMode){
				unit.AddBuff(buff);
			}
			else if(!buff.wrath){
				unit.AddBuff(buff);
				// Debug.Log ("Buff Added");
			}
		}
	}
}

[System.Serializable]
public class Buff{
	//For All Buffs
	public int ID;
	public BuffType buffType;
	public int duration;
	public EffectType effectType;
	public int strength;
	public bool wrath;
	public TargetType targetType;
	public bool permanent;
	
	//For Stat Buffs
	public bool oneShot;

	//For Adjacent Buffs
	public AdjacencyType adjType;
	public List<UnitType> adjUnits = new List<UnitType>();

	//For Terrain Buffs
	public bool isBio;
	public TerrainType terType;
	public BiomeType bioType;

	public Buff(int id, BuffType bt, int dur, EffectType et, int str, bool wrt, TargetType targ, bool perm, bool os, AdjacencyType at, List<UnitType> ut, bool ib, TerrainType tt, BiomeType bi){
		ID = id;
		buffType = bt;
		duration = dur;
		effectType = et;
		strength = str;
		wrath = wrt;
		targetType = targ;
		permanent = perm;
		oneShot = os;
		adjType = at;
		adjUnits = ut;
		isBio = ib;
		terType = tt;
		bioType = bi;
	}

	public void ChangeValue(Unit unt, bool add){	//'add' is if the effect is being added to or removed from a unit
		int multiplier;
		GameObject tempText = null;
		string operatorString;
		
		if(add){
			multiplier = 1;
			operatorString = "+ ";
		}
		else{
			multiplier = -1;
			operatorString = "";
		}
		
		switch(effectType){
		case EffectType.Attack:
			unt.AttackModifier += strength * multiplier;

//			tempText = MonoBehaviour.Instantiate(Logic.Inst.buffText,(unt.gameObject.transform.position + Vector3.up * Logic.Inst.offsetDist),Quaternion.identity) as GameObject;
//			tempText.GetComponent<TextMesh>().text = operatorString + (strength * multiplier);
			break;
			
		case EffectType.Defense:
			unt.DefenseModifier += strength * multiplier;

//			tempText = MonoBehaviour.Instantiate(Logic.Inst.buffText,(unt.gameObject.transform.position + Vector3.up * Logic.Inst.offsetDist),Quaternion.identity) as GameObject;
//			tempText.GetComponent<TextMesh>().text = operatorString + (strength * multiplier);
			break;
			
		case EffectType.Health:
			unt.CurrentHitpoints += strength * multiplier;
			tempText = MonoBehaviour.Instantiate(Logic.Inst.healText,(unt.gameObject.transform.position + Vector3.up * Logic.Inst.offsetDist),Quaternion.identity) as GameObject;
			tempText.GetComponent<TextMesh>().text = operatorString + (strength * multiplier);

			if(unt.CurrentHitpoints > unt.maxHitpoints){
				tempText.GetComponent<TextMesh>().text = operatorString + ((strength * multiplier) - (unt.CurrentHitpoints - unt.maxHitpoints));
				unt.CurrentHitpoints = unt.maxHitpoints;
			}
			break;
			
		case EffectType.Move:
			unt.CurrentMovePoints += strength * multiplier;

//			tempText = MonoBehaviour.Instantiate(Logic.Inst.buffText,(unt.gameObject.transform.position + Vector3.up * Logic.Inst.offsetDist),Quaternion.identity) as GameObject;
//			tempText.GetComponent<TextMesh>().text = operatorString + (strength * multiplier);
//			break;

		case EffectType.MaxMove:
			unt.movePoints += strength * multiplier;

//			tempText = MonoBehaviour.Instantiate(Logic.Inst.buffText,(unt.gameObject.transform.position + Vector3.up * Logic.Inst.offsetDist),Quaternion.identity) as GameObject;
//			tempText.GetComponent<TextMesh>().text = operatorString + (strength * multiplier);
			break;
			
		case EffectType.Range:
			unt.attackRange += strength * multiplier;

//			tempText = MonoBehaviour.Instantiate(Logic.Inst.buffText,(unt.gameObject.transform.position + Vector3.up * Logic.Inst.offsetDist),Quaternion.identity) as GameObject;
//			tempText.GetComponent<TextMesh>().text = operatorString + (strength * multiplier);
			break;
			
		case EffectType.Damage:
			unt.CurrentHitpoints -= strength * multiplier;

			tempText = MonoBehaviour.Instantiate(Logic.Inst.damageText,(unt.gameObject.transform.position + Vector3.up * Logic.Inst.offsetDist),Quaternion.identity) as GameObject;
			tempText.GetComponent<TextMesh>().text = "- " + (strength * multiplier);

			if(unt.CurrentHitpoints <= 0){
				unt.UnitKilled();
			}
			break;
			
		case EffectType.Hit:
			unt.HitModifier += strength * multiplier;

//			tempText = MonoBehaviour.Instantiate(Logic.Inst.buffText,(unt.gameObject.transform.position + Vector3.up * Logic.Inst.offsetDist),Quaternion.identity) as GameObject;
//			tempText.GetComponent<TextMesh>().text = operatorString + (strength * multiplier);
			break;
			
		case EffectType.Dodge:
			unt.DodgeModifier += strength * multiplier;

//			tempText = MonoBehaviour.Instantiate(Logic.Inst.buffText,(unt.gameObject.transform.position + Vector3.up * Logic.Inst.offsetDist),Quaternion.identity) as GameObject;
//			tempText.GetComponent<TextMesh>().text = operatorString + (strength * multiplier);
			break;
			
		default:
			Debug.Log("Something went terribly wrong with this effect.");
			break;
		}
	}
}