using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BalanceBar : MonoBehaviour {

	Slider bar;
	public Image background;
	public Image fill;

	// Use this for initialization
	void Start () {
		bar = GetComponentInChildren<Slider>();
		bar.value = Logic.Inst.Players[0].capturedAltars.Count;
		background.color = Logic.Inst.Players[1].playerColour;
		fill.color = Logic.Inst.Players[0].playerColour;
	}
	
	// Update is called once per frame
	void Update () {
		bar.value = Logic.Inst.Players[0].capturedAltars.Count;
	}
}
