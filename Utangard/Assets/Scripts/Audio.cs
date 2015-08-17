using UnityEngine;
using System.Collections.Generic;

public class Audio : MonoBehaviour {

	private AudioSource source;

	public List<SFXObj> sfx;

	public void Start() {
		source.GetComponent<AudioSource>();
	}

	public void PlaySFX(SFX sound) {
		source.PlayOneShot(sfx.Find(item=>item.sound == sound).clip);
	}
}

[System.Serializable]
public struct MusicObj{
	public Music song;
	public AudioClip clip;
}

[System.Serializable]
public struct SFXObj {
	public SFX sound;
	public AudioClip clip;
}