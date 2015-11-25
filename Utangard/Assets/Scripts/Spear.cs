using UnityEngine;
using System.Collections;

public class Spear : MonoBehaviour {
	
	public float velocity = 1.0f; // in m/s

	private Vector3 startPoint;
	private Vector3 endPoint;
	private float arcHeight;
	private float percent = 0.0f;

	// private float dist;

	public void CalcArc(Vector3 endPoint, float arcHeight){
		this.startPoint = transform.position;
		this.endPoint = endPoint;
		this.arcHeight = arcHeight;

		this.endPoint.y = this.startPoint.y;

		// dist = Vector3.Distance(transform.position, endPoint)

		transform.LookAt(endPoint);
	}

	public void Update() {
		Vector3 pos = Vector3.zero;

		percent += Time.deltaTime * velocity;

		if(percent > 1.0f){
			Destroy(this.gameObject);
		}

		//Previous calc, wasn't working slerping instead

		// pos.x = X(startPoint.x, endPoint.x, percent);
		// pos.y = Y(startPoint.y, arcHeight, percent);
		// pos.z = Z(startPoint.z, endPoint.z, percent);

		pos = Vector3.Slerp(transform.position, endPoint, percent);

		transform.position = pos;
	}

	// 	                                                              a                                                                                          
	// 	                                    db                      ,888,    ad888888b,                  88888888ba                                 ,ad8888ba,   
	// 	                                   d88b                   ,d8P"Y8b, d8"     "88        aa        88      "8b                    aa         d8"'    `"8b  
	// 	               8888888888         d8'`8b                 I8"     "8I        a8P        88        88      ,8P                    88        d8'            
	// 	8b       d8                      d8'  `8b     8b,     ,d8                ,d8P"     aaaa88aaaa    88aaaaaa8P' 8b,     ,d8    aaaa88aaaa    88             
	// 	`8b     d8'    8888888888       d8YaaaaY8b     `Y8, ,8P'               a8P"        """"88""""    88""""""8b,  `Y8, ,8P'     """"88""""    88             
	// 	 `8b   d8'                     d8""""""""8b      )888(               a8P'              88        88      `8b    )888(           88        Y8,            
	// 	  `8b,d8'                     d8'        `8b   ,d8" "8b,            d8"                ""        88      a8P  ,d8" "8b,         ""         Y8a.    .a8P  
	// 	    Y88'                     d8'          `8b 8P'     `Y8           88888888888                  88888888P"  8P'     `Y8                    `"Y8888Y"'   
	// 	    d8'                                                                                                                                                  
	// 	   d8'                                                                                                                                                   

	public float X(float start, float end, float t){
		float a = (end - start);
		float b = start;
		
		return a*t + b;
	}

	public float Z(float start, float end, float t){
		float a = (end - start);
		float b = start;

		return a*t + b;
	}

	public float Y(float start, float end, float t){
		float b = (end - start)/4;
		float a = -b;
		float c = start;

		return a*t*t + b * t * c;
	}
}
