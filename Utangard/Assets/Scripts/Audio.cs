using UnityEngine;
using System.Collections.Generic;

public class Audio : MonoBehaviour {

	private AudioSource source;

	public List<SFXObj> sfx;

	public void Start() {
		source = GetComponentInChildren<AudioSource>();
	}

	public void PlaySFX(SFX sound) {
		AudioClip[] clips = sfx.Find(item=>item.sound == sound).clips; 
		source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
	}

	public void OnUnitDeath(Unit unit, Unit killer){
		if(killer.type == UnitType.Swordsmen){
			PlaySFX(SFX.WomanKill);
		}
		else if(killer.type == UnitType.Hero && (killer.Owner.hero.type == HeroType.Eir || killer.Owner.hero.type == HeroType.Skadi)){
			PlaySFX(SFX.WomanKill);
		}
		else{
			PlaySFX(SFX.MaleKill);
		}

		if(unit.type == UnitType.Swordsmen){
			PlaySFX(SFX.WomanDeath);
		}
		else if(unit.type == UnitType.Hero && (unit.Owner.hero.type == HeroType.Eir || unit.Owner.hero.type == HeroType.Skadi)){
			PlaySFX(SFX.WomanDeath);
		}
		else{
			PlaySFX(SFX.MaleDeath);
		}
	}

	public void OnUnitAttack(Unit Attacker, Unit Defender){

	}

	public void OnUnitMove(){

	}

	public void OnGameEnd(){

	}

    public AudioSource Source()
    {
        return source;
    }
}

[System.Serializable]
public struct SFXObj {
	public SFX sound;
	public AudioClip[] clips;
}