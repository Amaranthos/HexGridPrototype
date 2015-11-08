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
	public GameObject skillParticle;
	
	public Skill(AbilityType at, AimType tt, bool hf, int cst, int crng, int arng, List<Target> targs, PassiveType pass, Unit hro, List<UnitType> afd, List<Buff> buf, GameObject skp){
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
		skillParticle = skp;
	}

	public void ApplyBuffSingle(CubeIndex index){
		Unit unit = Logic.Inst.Grid.TileAt(index).OccupyingUnit;

		if(!hitFoe && unit.Owner == hero.Owner && affected.Contains(unit.type)){
			AddBuffs(unit);
		}
		else if (hitFoe && unit.Owner != hero.Owner && affected.Contains(unit.type)){
			AddBuffs(unit);
		}
		SpawnParticle(unit);	
	}

	public void ApplyBuffAll(int playerNo){
		for(int i = 0; i < Logic.Inst.Players[playerNo].army.Count; i++){
			if(affected.Contains(Logic.Inst.Players[playerNo].army[i].type)){	//Check if the unit type is affected by this ability
				AddBuffs(Logic.Inst.Players[playerNo].army[i]);
				SpawnParticle(Logic.Inst.Players[playerNo].army[i]);			
			}
		}
	}

	public void ApplyBuffAoE(CubeIndex tileIndex){
		List<Tile> inRange = new List<Tile>();
		
		inRange = Logic.Inst.Grid.TilesInRange(tileIndex,AoERange);
		for(int i = 0; i < inRange.Count; i++){
			Unit hitUnit = inRange[i].OccupyingUnit;

			if(!hitFoe){
				if(hitUnit && hitUnit.Owner == hero.Owner && affected.Contains(hitUnit.type)){
					AddBuffs(hitUnit);
					SpawnParticle(hitUnit);				}
			}
			else{
				if(hitUnit && hitUnit.Owner != hero.Owner && affected.Contains(hitUnit.type)){
					AddBuffs(hitUnit);
					SpawnParticle(hitUnit);
				}
			}
		}
	}
	
	public void AddBuffs(Unit unit){
		for(int i = 0; i < buffs.Count; i++){
			if(buffs[i].wrath && hero.Owner.wrathMode){
				unit.AddBuff(buffs[i]);
			}
			else if(!buffs[i].wrath){
				unit.AddBuff(buffs[i]);
			}
		}

		unit.StartCoroutine("SpawnBuffText",unit.buffsToSpawn);
	}

	public void SpawnParticle(Unit unit){
		// GameObject tempobj = Logic.Instantiate(skillParticle,unit.transform.position,Quaternion.identity) as GameObject;
		// tempobj.GetComponent<AbilityParticles>().FadeParticle(tempobj);
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
	public bool procced;
	public bool debuff;
	
	//For Stat Buffs
	public bool oneShot;

	//For Adjacent Buffs
	public AdjacencyType adjType;
	public List<UnitType> adjUnits = new List<UnitType>();
	public int timesProcced;

	//For Terrain Buffs
	public bool isBio;
	public TerrainType terType;
	public BiomeType bioType;

	public Buff(int id, BuffType bt, int dur, EffectType et, int str, bool wrt, TargetType targ, bool perm, bool proc, bool db, bool os, AdjacencyType at, List<UnitType> ut,int tp,bool ib, TerrainType tt, BiomeType bi){
		ID = id;
		buffType = bt;
		duration = dur;
		effectType = et;
		strength = str;
		wrath = wrt;
		targetType = targ;
		permanent = perm;
		procced = proc;
		debuff = db;
		oneShot = os;
		adjType = at;
		adjUnits = ut;
		timesProcced = tp;
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
			break;
			
		case EffectType.Defense:
			unt.DefenseModifier += strength * multiplier;
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
			break;

		case EffectType.MaxMove:
			unt.movePoints += strength * multiplier;
			break;
			
		case EffectType.Range:
			unt.attackRange += strength * multiplier;
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