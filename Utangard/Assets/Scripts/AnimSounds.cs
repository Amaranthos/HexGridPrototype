using UnityEngine;
using System.Collections;

public class AnimSounds : MonoBehaviour {

	void Start () {
	
	}
	
	void Update () {
	
	}

	public void PlaySFX(SFX sfx){
		Logic.Inst.Audio.PlaySFX(sfx);
	}
}
