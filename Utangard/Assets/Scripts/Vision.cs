using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class Vision : MonoBehaviour {

	public float moveSpeed;
	public PairInt zoomRange;
	public float zoomSpeed;

	private Camera cam;
	private float camSize;

	private float minX;
	private float maxX;
	private float minY;
	private float maxY;

	private float mapX;
	private float mapY;

	private void Start() {
		cam = GetComponent<Camera>();
		camSize = cam.orthographicSize;

		mapX = Logic.Inst.Grid.gridSize.x * (Logic.Inst.Grid.hexRadius + 0.6f);
		mapY = Logic.Inst.Grid.gridSize.y * (Logic.Inst.Grid.hexRadius + 0.7f);

		CalculateCameraBounds();
	}

	private void Update() {
		
		Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
		move.Normalize();

		transform.position += move * moveSpeed;

		float currSize = camSize - Input.GetAxis("Mouse ScrollWheel");

		if (zoomRange.x > currSize || currSize < zoomRange.y)
			camSize = currSize;

		if (cam.orthographicSize >= zoomRange.x && cam.orthographicSize <= zoomRange.y)
			cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, camSize, Time.deltaTime * zoomSpeed);
		else
			if (cam.orthographicSize < zoomRange.x) {
				cam.orthographicSize = zoomRange.x + Mathf.Epsilon;
				camSize = cam.orthographicSize;
			}
	}

	private void LateUpdate() {

		Vector3 pos = transform.position;

		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.z = Mathf.Clamp(pos.z, minY, maxY);

		transform.position = pos;

		CalculateCameraBounds();
	}

	private void CalculateCameraBounds() {
		float vert = Camera.main.orthographicSize;
		float hor = vert * Screen.width / Screen.height;

		minX = Mathf.Min(hor - mapX / 2.0f, mapX / 2.0f - hor);
		maxX = Mathf.Max(hor - mapX / 2.0f, mapX / 2.0f - hor);

		minY = Mathf.Min(vert - mapY / 2.0f, mapY / 2.0f - vert);
		maxY = Mathf.Max(vert - mapY / 2.0f, mapY / 2.0f - vert);
	}
}
