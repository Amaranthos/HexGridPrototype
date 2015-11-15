using UnityEngine;
using System.Collections;

public class FireplaceLight : MonoBehaviour {

	Light fireLight;
	public Light lightToCopy;
	public bool copyLight = false;
	public float minFlickerIntesity = 7.5f;
	public float maxFlickerIntesity = 8f;
	public float flickerSpeed = 0.035f;
	public Color firstColor;
	public Color secondColor;

	int randomiser = 0;
	float counter;
	// Use this for initialization
	void Start () {
		fireLight = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!copyLight){
			if(counter > flickerSpeed){
				if(randomiser == 0){
					fireLight.intensity = (Random.Range(minFlickerIntesity, maxFlickerIntesity));
				}
				else{
					fireLight.intensity = (Random.Range(minFlickerIntesity, maxFlickerIntesity));
				}
				randomiser = Random.Range(0,1);
				counter = 0;
				if(fireLight.color == firstColor){
					fireLight.color = Color.Lerp(firstColor, secondColor, 8);
				}
				else{
					fireLight.color = Color.Lerp(secondColor, firstColor, 8);
				}
			}
			else{
				counter += Time.deltaTime;
			}
		}
		else{
			fireLight.color = lightToCopy.color;
			fireLight.intensity = lightToCopy.intensity;
		}
	}
}
