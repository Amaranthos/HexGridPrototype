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
	private MusicBaseState theme = MusicBaseState.None;

	void Awake() {
		quarterNote = 60 / bpm;
		timeIn = quarterNote * 8;
		timeOut = quarterNote * 32;
	}

	public void ChangeBase(MusicBaseState theme){
		if(this.theme == theme){
			return;
		}

		this.theme = theme;

		switch(theme){
			case MusicBaseState.Title:
				StartCoroutine(FadeIn("Title"));
				break;

			case MusicBaseState.Placing:
				StartCoroutine(FadeIn("Placing"));
				break;

			case MusicBaseState.Battle:
				StartCoroutine(FadeIn("Battle"));
				break;

			case MusicBaseState.NearWin:
				StartCoroutine(FadeIn("Winning"));
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

	private IEnumerator FadeIn(string inGroup){

		float timer = 0.0f;
		float vol;

		while(timer < timeIn){
			timer += Time.deltaTime;
			vol = Mathf.Lerp(-80.0f, 0.0f, timer/timeIn);
			master.SetFloat(inGroup, vol);
			yield return new WaitForEndOfFrame();
		}
		if(current != ""){
			StartCoroutine(FadeOut(current));
		}
		current = inGroup;
		yield return null;
	}

	private IEnumerator FadeOut(string outGroup) {

		float timer = 0.0f;
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