using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

	public AudioMixerSnapshot title;
	public AudioMixerSnapshot placing;
	public AudioMixerSnapshot battle;
	public AudioMixerSnapshot winning;

	public AudioMixerSnapshot mute;

	private float bpm = 128;
	private float timeIn = 0;
	private float timeOut = 0;
	private float quaterNote = 0;

	private AudioMixerSnapshot current;

	void Awake() {
		quaterNote = 60 / bpm;
		timeIn = quaterNote;
		timeOut = quaterNote * 32;
	}

	public void ChangeBase(MusicBaseState theme){
		switch(theme){
			case MusicBaseState.Title:
				current = title;
				break;

			case MusicBaseState.Placing:
				current = placing;
				break;

			case MusicBaseState.Battle:
				current = battle;
				break;

			case MusicBaseState.NearWin:
				current = winning;
				break;
		}

		current.TransitionTo(timeIn);
	}

	public void Mute(){
		mute.TransitionTo(0f);
	}
}

[System.Serializable]
public enum MusicBaseState {
	Title,
	Placing,
	Battle,
	NearWin
}