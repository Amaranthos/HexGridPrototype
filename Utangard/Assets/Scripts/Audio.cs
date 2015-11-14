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
}

[System.Serializable]
public struct SFXObj {
	public SFX sound;
	public AudioClip[] clips;
}