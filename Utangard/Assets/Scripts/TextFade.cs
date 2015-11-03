using UnityEngine;
using System.Collections;

public class TextFade : MonoBehaviour {
	public float moveSpeed = 1, fadeSpeed = 1, currentAlpha;
	private TextMesh damageText;

	void Start () {
		Destroy(gameObject,3f);
		damageText = gameObject.GetComponent<TextMesh>();
		currentAlpha = 1;
	}

	void Update () {
		transform.Translate(transform.up * Time.deltaTime);
		currentAlpha = Mathf.Lerp(currentAlpha,0,fadeSpeed);
		damageText.color = new Color(damageText.color.r,damageText.color.g,damageText.color.b,currentAlpha);
		transform.LookAt(-Camera.main.transform.position);
	}
}