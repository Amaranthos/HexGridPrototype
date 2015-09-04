using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class Vision : MonoBehaviour {

	public float moveSpeed;
	public PairInt zoomRange;

	private Camera cam;

	private void Start() {
		cam = GetComponent<Camera>();
	}

	private void Update() {
		
		Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
		move.Normalize();

		transform.position += move * moveSpeed;

		if (Input.GetAxis("Mouse ScrollWheel") > 0)
			cam.orthographicSize = Mathf.Max(cam.orthographicSize - 1, zoomRange.x);

		if (Input.GetAxis("Mouse ScrollWheel") < 0)
			cam.orthographicSize = Mathf.Min(cam.orthographicSize + 1, zoomRange.y);
	}
}
