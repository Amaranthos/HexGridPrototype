using UnityEngine;
using System.Collections;

public class MenuRandomiser : MonoBehaviour {

	public Material[] mats; // 1 - Eir, 2 - Heimy, 3 - Skadi, 4 - Thor
	public GameObject[] heroModels;
	public GameObject banner;

	// Use this for initialization
	void Start () {
		int h = Random.Range(0,heroModels.Length);
		for(int i = 0; i < heroModels.Length; i++){
			if(i == h){
				heroModels[i].SetActive(true);
			}
			else{
				heroModels[i].SetActive(false);
			}
		}

		for(int i = 0; i < mats.Length; i++){
			if(i == h){
				banner.GetComponent<SkinnedMeshRenderer>().material = mats[i];
			}
		}
	}
}
