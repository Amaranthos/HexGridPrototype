using UnityEngine;
using UnityEngine.UI;

public class ArmySelect : MonoBehaviour {
	public ArmySelectGuiHolder[] army;

	
	public void Start() {
		for (int i = 0; i < Logic.Inst.Players.Length; i++) {
			army[i].Player = Logic.Inst.Players[i];
			army[i].Init();
		}
	}

	public void Update() {
		for (int i = 0; i < army.Length; i++)
			army[i].UpdateTitle();
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

	public void UpdateTitle() {
		title.text = Player.playerName + " Food: " + Player.CurrentFood;
	}
}

[System.Serializable]
public struct UnitCountGuiHolder {
	public int count;
	public Text text;
	public Button neg;
	public Button pos;


	public Player Player { get; set;}
	public int Cost { get; set; }

	public void Init() {
		UnitCountGuiHolder holder = this;

		neg.onClick.AddListener(() => {
			holder.Decrement();
		});

		pos.onClick.AddListener(() => {
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