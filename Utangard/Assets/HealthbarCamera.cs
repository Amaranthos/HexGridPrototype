using UnityEngine;
using System.Collections;

public class HealthbarCamera : MonoBehaviour {

	public Camera mainCam;
	Camera cam;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		cam.fieldOfView = mainCam.fieldOfView;
		transform.position = mainCam.transform.position;
		transform.rotation = mainCam.transform.rotation;
	
	}
}
