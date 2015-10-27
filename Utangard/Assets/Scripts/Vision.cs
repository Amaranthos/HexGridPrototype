using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class Vision : MonoBehaviour {

	public float moveSpeed;
	public Vector2 zoomRange;
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

		mapX = Logic.Inst.Grid.mapWidth * (Logic.Inst.Grid.hexRadius);
		mapY = Logic.Inst.Grid.mapHeight * (Logic.Inst.Grid.hexRadius);

		CalculateCameraBounds();
	}

	private void Update() {
		
		Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
		move.Normalize();

		transform.localPosition += move * moveSpeed;

		
		float input = Input.GetAxis("Mouse ScrollWheel");

		if(input != 0.0f){
			StopCoroutine(Zoom(input));
			StartCoroutine(Zoom(input));
		}
	}

	private IEnumerator Zoom(float input){
		float targetFOV = cam.fieldOfView - input * zoomSpeed;

		while(cam.fieldOfView != targetFOV && cam.fieldOfView >= zoomRange.x && cam.fieldOfView <= zoomRange.y){
			cam.fieldOfView -= input;
			yield return new WaitForEndOfFrame();
		}

		if(cam.fieldOfView > zoomRange.y)
			cam.fieldOfView = zoomRange.y;

		if(cam.fieldOfView < zoomRange.x - 0.01f)
			cam.fieldOfView = zoomRange.x;
		
		yield return null;
	}

	private void LateUpdate() {

		Vector3 pos = transform.localPosition;

		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.z = Mathf.Clamp(pos.z, minY, maxY);

		transform.localPosition = pos;

		CalculateCameraBounds();
	}

	private void CalculateCameraBounds() {
		float vert = cam.fieldOfView;
		float hor = vert * Screen.width / Screen.height;

		minX = Mathf.Min(hor - mapX / 2.0f, mapX / 2.0f - hor);
		maxX = Mathf.Max(hor - mapX / 2.0f, mapX / 2.0f - hor);

		minY = Mathf.Min(vert - mapY / 2.0f, mapY / 2.0f - vert);
		maxY = Mathf.Max(vert - mapY / 2.0f, mapY / 2.0f - vert);

		// These values are hard coded, bad I know, but this is kind of the only way I know how to at the moment

		float hScalar = Scalar(7, 30, zoomRange.x, zoomRange.y, vert);

		minX /= hScalar;
		maxX /= hScalar;

		minY /= Scalar(1.25f, 4f, zoomRange.x, zoomRange.y, vert);
		maxY /= 10;
		maxY -= 5;
	}

	private float Scalar(float a, float b, float min, float max, float fov){
		return ((b-a)*(fov - min)/(max-min)) + a;
	}
}
