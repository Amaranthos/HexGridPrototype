using UnityEngine;
using System.Collections;

public class Spear : MonoBehaviour {
	
	public float velocity = 1.0f; // in m/s

	public Vector3 endPoint;
	public Vector3 startPoint;
	private float percent = 0.0f;

	public void Update() {
		percent += Time.deltaTime * velocity;

		if(percent > 1.0f){
			Destroy(this.gameObject, 0.5f);
		}

		transform.position = Vector3.Lerp(startPoint, endPoint, percent);
	}
}