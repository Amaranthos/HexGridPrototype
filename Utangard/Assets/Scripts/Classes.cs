using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Effect{
	
	public EffectType type;
	public int duration;
	public int strength;
	public bool oneShot;
	public bool wrath;
	
	public Effect(EffectType et, int dur, int str, bool os, bool wrt){
		type = et;
		duration = dur;
		strength = str;
		oneShot = os;
		wrath = wrt;
	}

	public void ChangeValue(Unit unt, bool add){	//'add' is if the effect is being added to or removed from a unit
		int multiplier;

		if(add){
			multiplier = 1;
		}
		else{
			multiplier = -1;
		}

		switch(type){
		case EffectType.Attack:
			unt.AttackModifier += strength * multiplier;
			break;

		case EffectType.Defense:
			unt.DefenseModifier += strength * multiplier;
			break;

		case EffectType.Health:
			unt.CurrentHitpoints += strength * multiplier;
			break;

		case EffectType.Move:
			unt.movePoints += strength * multiplier;
			break;

		case EffectType.Range:
			unt.attackRange += strength * multiplier;
			break;

		case EffectType.Damage:
			unt.CurrentHitpoints -= strength * multiplier;
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

[System.Serializable]
public class Ability{

	public TargetType target;	//When in wrath mode, some abilities have an added AoE effect. May need to apply AoE to effects, rather than to abilites.
	public bool hitFoe;
	public int cost;
	public int range;
	public bool passive;
	public Unit hero;
	public List<UnitType> affected = new List<UnitType>();
	public List<Effect> effects = new List<Effect>();

	public Ability(TargetType tt, bool hf, int cst, int rng, bool pass, Unit hro, List<UnitType> afd, List<Effect> efs){
		target = tt;
		hitFoe = hf;
		cost = cst;
		range = rng;
		passive = pass;
		hero = hro;
		affected = afd;
		effects = efs;
	}
	//Activation still needs to be finished/improved. With targeting in particular.
	public void Activate(Unit unt){		//Should pass hero unit if target is all, or targeted unit if single/AoE
		Player enemy;

		if(hero.Owner == Logic.Inst.Players[0]){
			enemy = Logic.Inst.Players[1];
		}
		else{
			enemy = Logic.Inst.Players[0];
		}

		if(target == TargetType.All){
			if(!hitFoe){
				foreach(Unit unit in hero.Owner.army){
					if(affected.Contains(unit.type))	//Check if the unit type is affected by this ability
						ApplyEffects(unit);
				}
			}
			else{
				foreach(Unit unit in enemy.army){
					if(affected.Contains(unit.type))	//Check if the unit type is affected by this ability
						ApplyEffects(unit);
				}
			}
		}
		else if(target == TargetType.Single){
			//Check if the unit is friendly or not and if the ability targets friendly units or not.
			if(!hitFoe && unt.Owner == hero.Owner && affected.Contains(unt.type)){
				ApplyEffects(unt);
			}
			else if (hitFoe && unt.Owner != hero.Owner && affected.Contains(unt.type)){
				ApplyEffects(unt);
			}
		}
		else{
			//Need to determine how to do AoE Targeting. This is going to be complex...
		}
	}

	public void ApplyEffects(Unit unt){
		foreach (Effect eft in effects){
			if(eft.wrath && hero.Owner.wrathMode){ //Determines if wrath effects should be applied.
				unt.AddEffect(eft);
			}
			else if(!eft.wrath){
				unt.AddEffect(eft);
			}
		}
	}
}
