﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour {
	List<AbilityParticles> currentParticles = new List<AbilityParticles>();
	List<Skill> currentSkills = new List<Skill>();
	List<int> partsToRemove = new List<int>();
	List<int> skillsToRemove = new List<int>();

	void Start () {
	
	}

	void Update () {
	
	}

	public void CreateParticle(Skill skill, Unit unit){
		bool isOneShot = true;
		GameObject tempobj = Instantiate(skill.skillParticle,unit.transform.position,Quaternion.identity) as GameObject;
		tempobj.transform.parent = unit.transform;

		AbilityParticles partScript;
		partScript = tempobj.GetComponent<AbilityParticles>();
		partScript.FadeIn(tempobj);

		for(int i = 0; i < skill.buffs.Count; i++){
			if((unit.Owner.wrathMode && skill.buffs[i].wrath && !skill.buffs[i].oneShot) || (!skill.buffs[i].wrath && !skill.buffs[i].oneShot)){
				isOneShot = false;
			}
		}

		if(isOneShot){
			partScript.FadeOut();
		}
		else{
			currentParticles.Add(tempobj.GetComponent<AbilityParticles>());
			currentSkills.Add(skill);
			RemoveParticle(skill);
		}
	}

	public void RemoveParticle(Skill skill){
		partsToRemove.Clear();
		skillsToRemove.Clear();

		for(int i = 0; i < currentSkills.Count; i++){
			if(currentSkills[i] == skill){
				for(int j = 0; j < currentParticles.Count; j++){
					if(currentParticles[j].ID == currentSkills[i].skillParticle.GetComponent<AbilityParticles>().ID){
						currentParticles[j].FadeOut();
						partsToRemove.Add(j);
					}
				}
				skillsToRemove.Add(i);
			}
		}

//		for(int x = skillsToRemove.Count-1; x > 0; x--){
//			print("REMOVING SKILL");
//			currentSkills.RemoveAt(skillsToRemove[x]);
//		}
//
//		for(int y = partsToRemove.Count-1; y > 0; y--){
//			print ("REMOVING PARTICLE");
//			currentParticles.RemoveAt(partsToRemove[y]);
//		}
	}
}
