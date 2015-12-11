using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

	public AudioMixer master;

	private float bpm = 128;
	private float timeIn = 0;
	private float timeOut = 0;
	private float quarterNote = 0;

	private string current = "";
	public string currentWinner = "";
	private string oldWinner = "";
	private MusicBaseState theme = MusicBaseState.None;

	void Awake() {
		quarterNote = 60 / bpm;
		timeIn = quarterNote * 8;
		timeOut = quarterNote * 32;
	}

	public void RestartTracks() {
		foreach(AudioSource source in GetComponents<AudioSource>()) {
			source.Stop();
			source.Play();
		}
	}

	public void ChangeBase(MusicBaseState theme){
		if(this.theme == theme){
			return;
		}

		this.theme = theme;

		switch(theme){
			case MusicBaseState.Title:
				StartCoroutine(FadeIn("Title"));

				Debug.Log("Title");

				if(oldWinner != ""){
					StartCoroutine(FadeOut(oldWinner));
				}
				break;

			case MusicBaseState.Placing:
				StartCoroutine(FadeIn("Placing"));

				if(oldWinner != ""){
					StartCoroutine(FadeOut(oldWinner));
				}
				break;

			case MusicBaseState.Battle:
				StartCoroutine(FadeIn("Battle"));
				
				if(oldWinner != ""){
					StartCoroutine(FadeOut(oldWinner));
				}
				break;

			case MusicBaseState.NearWin:
				StartCoroutine(FadeIn("Winning"));
				if(currentWinner != oldWinner){
					if(oldWinner != ""){
						StartCoroutine(FadeOut(oldWinner));
					}
					StartCoroutine(FadeIn(currentWinner, false));
				}
				break;

			case MusicBaseState.None:
				break;
		}
	}

	public void Mute(){
		master.SetFloat("Master", -80.0f);
	}

	public void Unmute() {
		master.SetFloat("Master", 0.0f);
	}

	private IEnumerator FadeIn(string inGroup, bool stopCurrent = true){
		float timer = 0.0f;
		float vol;

		while(timer < timeIn){
			timer += Time.deltaTime;
			vol = Mathf.Lerp(-80.0f, 0.0f, timer/timeIn);
			master.SetFloat(inGroup, vol);
			yield return new WaitForEndOfFrame();
		}

		if(stopCurrent){
			if(current != ""){
				StartCoroutine(FadeOut(current));
			}
			current = inGroup;
		}
		else {
			oldWinner = currentWinner;
		}
		yield return null;
	}

	private IEnumerator FadeOut(string outGroup) {		float timer = 0.0f;
		float vol;

		while(timer < timeOut){
			timer += Time.deltaTime;
			vol = Mathf.Lerp(0.0f, -80.0f, timer/timeOut);
			master.SetFloat(outGroup, vol);
			yield return new WaitForEndOfFrame();
		}
		yield return null;
	}
}

public enum MusicBaseState {
	Title,
	Placing,
	Battle,
	NearWin,

	None
}