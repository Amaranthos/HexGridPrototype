using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ArmySelect : MonoBehaviour {
	public ArmySelectGuiHolder[] army;
	public Toggle p1;
	public Toggle p2;
	public Button start;
		
	public void Start() {
		for (int i = 0; i < Logic.Inst.Players.Length; i++) {
			army[i].Player = Logic.Inst.Players[i];
			army[i].Init();
		}
	}

	public void Update() {
		for (int i = 0; i < army.Length; i++)
			army[i].UpdateTitle();

		if (p1.isOn && p2.isOn)
			start.interactable = true;
	}

	public void Finalise() {
		int[][] armies = new int[army.Length][];
		for (int i = 0; i < army.Length; i++) {
			armies[i] = army[i].Finalise().ToArray();
		}

		Logic.Inst.SetupGameWorld(armies);
		gameObject.SetActive(false);
	}
}

[System.Serializable]
public struct ArmySelectGuiHolder {
	public UnitCountGuiHolder[] units;

	public Text title;

	public Player Player { get; set; }

	public void Init() {
		for (int i = 0; i < units.Length; i++) {
			units[i].Player = Player;
			units[i].Cost = Logic.Inst.UnitList.GetUnit((UnitType)i).GetComponent<Unit>().cost;
			units[i].Init();
		}
	}

	public List<int> Finalise() {
		List<int> ret = new List<int>();

		for (int i = 0; i < units.Length; i++) { 
			for (int j = 0; j < units[i].count; j++) {
				ret.Add(i);
			}
		}
		return ret;
	}

	public void UpdateTitle() {
		title.text = Player.playerName + " Food: " + Player.CurrentFood;
	}
}

[System.Serializable]
public class UnitCountGuiHolder {
	public int count;
	public Text text;
	public Button neg;
	public Button pos;


	public Player Player { get; set;}
	public int Cost { get; set; }

	public void Init() {
		UnitCountGuiHolder holder = this;

		neg.onClick.AddListener(delegate {
			holder.Decrement();
		});

		pos.onClick.AddListener(delegate {
			holder.Increment();
		});
	}

	public void Increment() {
		if (Player.CurrentFood >= Cost){
			count++;
			Player.CurrentFood -= Cost;
			text.text = count.ToString();
		}
	}

	public void Decrement() {
		if (count > 0) {
			count--;
			Player.CurrentFood += Cost;
			text.text = count.ToString();
		}
	}
}