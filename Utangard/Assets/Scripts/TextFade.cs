using UnityEngine;
using System.Collections;

public class TextFade : MonoBehaviour {
	public float moveSpeed = 1, fadeSpeed = 1, currentAlpha;
	private TextMesh damageText;
	public Vector3 lookVect;
	public float rotX,rotY;

	void Start () {
		Destroy(gameObject,3f);
		damageText = gameObject.GetComponent<TextMesh>();
		currentAlpha = 1;
	}

	void Update () {
		transform.Translate(transform.up * Time.deltaTime);
		currentAlpha = Mathf.Lerp(currentAlpha,0,fadeSpeed);
		damageText.color = new Color(damageText.color.r,damageText.color.g,damageText.color.b,currentAlpha);

		lookVect = transform.position - Camera.main.transform.position;
		Vector3 horVect = new Vector3(lookVect.x,0,lookVect.z);
		rotX = Vector3.Angle(transform.forward,horVect);
		if(rotX > 10){
			transform.Rotate(transform.up,rotX);
		}
//		else{		//Broken as all hell from here.
//			rotY = Vector3.Angle(transform.forward,lookVect);
//			if(rotY > 5){
//				transform.Rotate(transform.right,rotY);
//			}
//		}
	}
}