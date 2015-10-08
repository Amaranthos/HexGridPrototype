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
		Unit unit = Logic.Inst.Grid.TileAt(index).OccupyngUnit;

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
				if(tile.OccupyngUnit && tile.OccupyngUnit.Owner == hero.Owner && affected.Contains(tile.OccupyngUnit.type)){
					AddBuffs(tile.OccupyngUnit);
					Debug.Log("Adding Buff");
				}
			}
			else{
				if(tile.OccupyngUnit && tile.OccupyngUnit.Owner != hero.Owner && affected.Contains(tile.OccupyngUnit.type)){
					AddBuffs(tile.OccupyngUnit);
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
				Debug.Log ("Buff Added");
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
	public TerrainType terType;

	public Buff(int id, BuffType bt, int dur, EffectType et, int str, bool wrt, TargetType targ, bool perm, bool os, AdjacencyType at, List<UnitType> ut, TerrainType tt){
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
		terType = tt;
	}

	public void ChangeValue(Unit unt, bool add){	//'add' is if the effect is being added to or removed from a unit
		int multiplier;
		
		if(add){
			multiplier = 1;
		}
		else{
			multiplier = -1;
		}
		
		switch(effectType){
		case EffectType.Attack:
			unt.AttackModifier += strength * multiplier;
			break;
			
		case EffectType.Defense:
			unt.DefenseModifier += strength * multiplier;
			break;
			
		case EffectType.Health:
			unt.CurrentHitpoints += strength * multiplier;
			if(unt.CurrentHitpoints > unt.maxHitpoints){
				unt.CurrentHitpoints = unt.maxHitpoints;
			}
			break;
			
		case EffectType.Move:
			unt.movePoints += strength * multiplier;
			break;
			
		case EffectType.Range:
			unt.attackRange += strength * multiplier;
			break;
			
		case EffectType.Damage:
			unt.CurrentHitpoints -= strength * multiplier;
			if(unt.CurrentHitpoints <= 0){
				unt.UnitKilled();
			}
			break;
			
		case EffectType.Hit:
			unt.HitModifier += strength * multiplier;
			break;
			
		case EffectType.Dodge:
			unt.DodgeModifier += strength * multiplier;
			break;
			
		default:
			Debug.Log("Something went terribly wrong with this effect.");
			break;
		}
	}
}