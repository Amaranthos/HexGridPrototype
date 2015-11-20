using UnityEngine;
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
//		if(Input.GetKeyDown(KeyCode.M)){	//For Testing Purposes Only
//			for(int i = 0; i < currentSkills.Count; i++){
//				RemoveParticle(currentSkills[i]);
//			}
//		}	
	}

	public void CreateParticle(Skill skill, Unit unit){
		bool isOneShot = true;
		GameObject tempobj = Instantiate(skill.skillParticle,unit.transform.position,skill.skillParticle.transform.rotation) as GameObject;
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
		}
	}

	public void RemoveParticle(int skillID){
		Skill skill = null;

		for(int k = 0; k < Logic.Inst.Players.Length; k++){	//Eurgh, this is ugly. But at the moment, it's the best way I've got.
			if(Logic.Inst.Players[k].hero.active1.ID == skillID){
				skill = Logic.Inst.Players[k].hero.active1;
			}
			else if(Logic.Inst.Players[k].hero.active2.ID == skillID){
				skill = Logic.Inst.Players[k].hero.active2;
			}
			else if(Logic.Inst.Players[k].hero.passive.ID == skillID){
				skill = Logic.Inst.Players[k].hero.passive;
			}
		}

		skillsToRemove.Clear();

		for(int i = 0; i < currentSkills.Count; i++){
			partsToRemove.Clear();
			
			if(currentSkills[i].ID == skill.ID){
				for(int j = 0; j < currentParticles.Count; j++){
					if(currentParticles[j].ID == currentSkills[i].skillParticle.GetComponent<AbilityParticles>().ID){
						currentParticles[j].FadeOut();
						partsToRemove.Add(j);
					}
				}

				for(int y = partsToRemove.Count-1; y > -1; y--){
					currentParticles.RemoveAt(partsToRemove[y]);
				}

				skillsToRemove.Add(i);
			}
		}

		for(int x = skillsToRemove.Count-1; x > -1; x--){
			currentSkills.RemoveAt(skillsToRemove[x]);
		}
	}
}
