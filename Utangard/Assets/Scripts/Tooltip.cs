using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoBehaviour {

	public Text textObj;
	public Vector2 tipOffset = new Vector2();
	RectTransform rect;
	Vector2 rectSize = new Vector2();
	Vector3 cursorPos = new Vector3(0,0,5);

	void Start () 
	{
		rect =  GetComponent<RectTransform>();
		cursorPos.x = Input.mousePosition.x + tipOffset.x;
		cursorPos.y = Input.mousePosition.y + tipOffset.y;
	}

	// Update is called once per frame
	void Update () 
	{
		cursorPos.x = Input.mousePosition.x + tipOffset.x;
		cursorPos.y = Input.mousePosition.y + tipOffset.y;
		if(textObj.text == "")
		{
			rect.sizeDelta = rectSize;
		}
		transform.position = Camera.main.ScreenToWorldPoint(cursorPos);
	}
}
