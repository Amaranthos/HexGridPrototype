using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CSVParser{
	public string GetCSV(string filePath)
	{
		string fileData = System.IO.File.ReadAllText(filePath);
		return fileData;
	}
}

[System.Serializable]
public class Target{
	public Unit unit;
	public CubeIndex Index, origin;
	public TargetType type;
	public bool needsUnit;
	public bool needsSpace;

	public Target (Unit unt, CubeIndex pint, TargetType tt, bool nu, bool ns){
		unit = unt;
		Index = pint;
		type = tt;
		needsUnit = nu;
		needsSpace = ns;
	}
}

//[System.Serializable]
//public class Effect{
//	
//	public EffectType type;
//	public int duration;
//	public int strength;
//    public int range;
//	public bool oneShot;
//	public bool wrath;
//	public bool skadiWrath;
//	
//	public Effect(EffectType et, int dur, int str, int rng, bool os, bool wrt, bool swt){
//		type = et;
//		duration = dur;
//		strength = str;
//        range = rng;
//		oneShot = os;
//		wrath = wrt;
//		skadiWrath = swt;
//	}
//
//	public void ChangeValue(Unit unt, bool add){	//'add' is if the effect is being added to or removed from a unit
//		int multiplier;
//
//		if(add){
//			multiplier = 1;
//		}
//		else{
//			multiplier = -1;
//		}
//
//		switch(type){
//		case EffectType.Attack:
//			unt.AttackModifier += strength * multiplier;
//			break;
//
//		case EffectType.Defense:
//			unt.DefenseModifier += strength * multiplier;
//			break;
//
//		case EffectType.Health:
//			unt.CurrentHitpoints += strength * multiplier;
//			if(unt.CurrentHitpoints > unt.maxHitpoints){
//				unt.CurrentHitpoints = unt.maxHitpoints;
//			}
//			break;
//
//		case EffectType.Move:
//			unt.movePoints += strength * multiplier;
//			break;
//
//		case EffectType.Range:
//			unt.attackRange += strength * multiplier;
//			break;
//
//		case EffectType.Damage:
//			unt.CurrentHitpoints -= strength * multiplier;
//			if(unt.CurrentHitpoints <= 0){
//				unt.UnitKilled();
//			}
//			break;
//
//		case EffectType.Hit:
//			unt.HitModifier += strength * multiplier;
//			break;
//
//		case EffectType.Dodge:
//			unt.DodgeModifier += strength * multiplier;
//			break;
//
//		default:
//			Debug.Log("Something went terribly wrong with this effect.");
//			break;
//		}
//	}
//}
//
//[System.Serializable]
//public class Ability{
//
//	public AimType target;	//When in wrath mode, some abilities have an added AoE effect. May need to apply AoE to effects, rather than to abilites.
//	public bool hitFoe;
//	public int cost;
//	public int range;
//    public int stages;
//	public PassiveType passive;
//	public Unit hero;
//	public List<UnitType> affected = new List<UnitType>();
//	public List<Effect> effects = new List<Effect>();
//
//	public Ability(AimType tt, bool hf, int cst, int rng, int stg, PassiveType pass, Unit hro, List<UnitType> afd, List<Effect> efs){
//		target = tt;
//		hitFoe = hf;
//		cost = cst;
//		range = rng;
//        stages = stg;
//		passive = pass;
//		hero = hro;
//		affected = afd;
//		effects = efs;
//	}
//	//Activation still needs to be finished/improved. With targeting in particular.
//	public void Activate(Unit unt){		//Should pass hero unit if target is all, or targeted unit if single/AoE
//		Player enemy;
//
//		if(hero.Owner == Logic.Inst.Players[0]){
//			enemy = Logic.Inst.Players[1];
//		}
//		else{
//			enemy = Logic.Inst.Players[0];
//		}
//
//		if(passive == PassiveType.None){
//			if(target == AimType.All){
//				if(!hitFoe){
//					foreach(Unit unit in hero.Owner.army){
//						if(affected.Contains(unit.type))	//Check if the unit type is affected by this ability
//							ApplyEffects(unit);
//					}
//				}
//				else{
//					foreach(Unit unit in enemy.army){
//						if(affected.Contains(unit.type))	//Check if the unit type is affected by this ability
//							ApplyEffects(unit);
//					}
//				}
//			}
//			else if(target == AimType.Single){
//				//Check if the unit is friendly or not and if the ability targets friendly units or not.
//				if(!hitFoe && unt.Owner == hero.Owner && affected.Contains(unt.type)){
//					ApplyEffects(unt);
//				}
//				else if (hitFoe && unt.Owner != hero.Owner && affected.Contains(unt.type)){
//					ApplyEffects(unt);
//				}
//			}
//			else{
//				ApplyEffects(unt);
//			}
//		}
//		else if(passive == PassiveType.OneShot){
//			ApplyEffects(unt);
//		}
//	}
//
//	public void ApplyEffects(Unit unt){
//		List<Tile> inRange = new List<Tile>();
//
//		foreach (Effect eft in effects){
//
//			if(eft.wrath && hero.Owner.wrathMode){ //Determines if wrath effects should be applied.
//				inRange = Logic.Inst.Grid.AbilityRange(unt.Index,eft.range);
//				foreach(Tile tile in inRange){
//					if(!hitFoe){
//						if(tile.OccupyngUnit && tile.OccupyngUnit.Owner == hero.Owner){
//							tile.OccupyngUnit.AddEffect(eft);
//						}
//					}
//					else{
//						if(tile.OccupyngUnit && tile.OccupyngUnit.Owner != hero.Owner){
//							tile.OccupyngUnit.AddEffect(eft);
//						}
//					}
//				}
//			}
//			else if(!eft.wrath){
//				inRange = Logic.Inst.Grid.AbilityRange(unt.Index,eft.range);
//				foreach(Tile tile in inRange){
//					if(!hitFoe){
//						if(tile.OccupyngUnit && tile.OccupyngUnit.Owner == hero.Owner){
//							tile.OccupyngUnit.AddEffect(eft);
//						}
//					}
//					else{
//						if(tile.OccupyngUnit && tile.OccupyngUnit.Owner != hero.Owner){
//							tile.OccupyngUnit.AddEffect(eft);
//						}
//					}
//				}
//			}
//		}
//	}
//
//	public void TeleportUnit(Unit unit, Tile tile){
//		//Logic.Inst.Audio.PlaySFX(SFX.Unit_Move);
//		Logic.Inst.Grid.GetTile(unit.Index).OccupyngUnit = null;
//		unit.Index = tile.Index;
//		tile.OccupyngUnit = unit;
//		
//		unit.transform.position = tile.transform.position;
//		
//		Altar altar = Logic.Inst.GetAltar(tile.Index);
//		
//		if (altar)
//			altar.PlayerCaptureAltar(unit.Owner);
//	}
//}
