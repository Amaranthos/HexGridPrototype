using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class Vision : MonoBehaviour {

	public float moveSpeed;
	public Vector2 zoomRange;
	public float zoomSpeed;
	public float horizSpinSpeed;
	public float vertSpinSpeed;
	public float vertMin;
	public float vertMax;
	private float origVertSpin;
	private float origHorizSpin;
	public float spinMultiplier;

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

		origVertSpin = vertSpinSpeed;
		origHorizSpin = horizSpinSpeed;
	}

	private void Update() {

		Vector3 moveX = transform.parent.right * Input.GetAxisRaw("Horizontal");
		moveX = new Vector3(moveX.x,0,moveX.z);
		Vector3 moveZ = transform.parent.forward * Input.GetAxisRaw("Vertical");
		moveZ = new Vector3(moveZ.x,0,moveZ.z);
		Vector3 move = moveX + moveZ;
		move.Normalize();

		transform.parent.localPosition += move * moveSpeed;



		if(Input.GetKey(KeyCode.Mouse2)){
			transform.parent.RotateAround(transform.position,Vector3.up,horizSpinSpeed*Input.GetAxisRaw("Mouse X")*Time.deltaTime);
			transform.parent.RotateAround(transform.parent.position,transform.parent.right,vertSpinSpeed*Input.GetAxisRaw("Mouse Y")*Time.deltaTime);
			
			Vector3 tempVect = transform.parent.rotation.eulerAngles;
			tempVect.x = ClampAngle(tempVect.x,vertMin,vertMax);
			
			transform.parent.eulerAngles = tempVect;
		}

		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			horizSpinSpeed = origHorizSpin * spinMultiplier;
			vertSpinSpeed = origVertSpin * spinMultiplier;
        }
		else{
			horizSpinSpeed = origHorizSpin;
			vertSpinSpeed = origVertSpin;
		}

		float input = Input.GetAxis("Mouse ScrollWheel");

		cam.fieldOfView -= zoomSpeed * 10 * input;
		cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, zoomRange.x, zoomRange.y);
		

		// if(input != 0.0f){
		// 	StopCoroutine(Zoom(input));
		// 	StartCoroutine(Zoom(input));
		// }
	}

	private IEnumerator Zoom(float input){
		float targetFOV = cam.fieldOfView - input * 10 * zoomSpeed;

		if(targetFOV < zoomRange.x || targetFOV > zoomRange.y){
			yield return null;
		}

		float diff = Mathf.Sign(targetFOV - cam.fieldOfView);

		if(diff == -1){
			while(cam.fieldOfView > targetFOV){
				cam.fieldOfView -= input * zoomSpeed;
				// ClampFOV();
				yield return new WaitForEndOfFrame();
			}
		}

		if(diff == 1){
			while(cam.fieldOfView < targetFOV){
				cam.fieldOfView -= input * zoomSpeed;
				// ClampFOV();
				yield return new WaitForEndOfFrame();
			}
		}	
		
		cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, zoomRange.x, zoomRange.y);	

		yield return null;
	}

	private float ClampAngle(float angle, float min, float max){
		if (angle<90 || angle>270){       // if angle in the critic region...
			if (angle>180) 
				angle -= 360;  // convert all angles to -180..+180
			if (max>180) 
				max -= 360;
			if (min>180) 
				min -= 360;
		}    
		angle = Mathf.Clamp(angle, min, max);
		if (angle<0) 
			angle += 360;  // if angle negative, convert to 0..360

        return angle;
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
