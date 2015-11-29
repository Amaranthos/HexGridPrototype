using UnityEngine;
using System.Collections;

public class SpearEvent : MonoBehaviour {
	public void TriggerThrow() {
		Debug.Log("Spear Throw triggered");
		transform.parent.GetComponent<Unit>().ThrowSpear();
	}
}
