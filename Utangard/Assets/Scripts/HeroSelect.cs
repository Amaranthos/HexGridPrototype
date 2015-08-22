using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeroSelect : MonoBehaviour {
	public HeroSelectGuiHolder[] heroes;
}

[System.Serializable]
public class HeroSelectGuiHolder {
	public Text player;
	public Text hero;
	public Text desc;
	public Button neg;
	public Button pos;

	public Hero selected;

	public void Init() {
		HeroSelectGuiHolder holder = this;

		neg.onClick.AddListener(delegate {
			holder.Decrement();
		});

		pos.onClick.AddListener(delegate {
			holder.Increment();
		});
	}

	public void Decrement() {

	}

	public void Increment() {

	}
}