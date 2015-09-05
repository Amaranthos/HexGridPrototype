using UnityEngine;
using System.Collections;

public class Altar : MonoBehaviour {
	[SerializeField]
	private Player owner = null;

	public PairInt Index { get; set; }

	public void PlayerCaptureAltar(Player player) {
		if (owner)
			owner.capturedAltars.Remove(this);
		owner = player;
		player.capturedAltars.Add(this);
	}
}
