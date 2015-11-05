using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityParticles : MonoBehaviour {
	private MeshRenderer[] mats;
	private ParticleSystem[] parts;
	private List<Color> matCols = new List<Color>();
	private List<Color> partCols = new List<Color>();
	private bool lerpingCols;
	private float alpha;
	private Color endCol;
	public float lerpSpeed;
	public float fadeTime;
	public float destroyTime;

	void Start () {
	
	}

	void Update () {
		if(lerpingCols){
			for(int i = 0; i < mats.Length; i++){
//				mats[i].materials[0].color = new Color(1,1,1,1);
			}
			
			for(int j = 0; j < parts.Length; j ++){
				parts[j].startColor = Color.Lerp(parts[j].startColor,partCols[j],lerpSpeed);
			}

			fadeTime -= Time.deltaTime;

			if(fadeTime <= 0){
				for(int x = 0; x < matCols.Count; x++){
					//Materials will lerp here when it actually works.
				}
				
				for(int y = 0; y < partCols.Count; y ++){
					partCols[y] = SetColClear(partCols[y]);
				}

				lerpingCols = false;

			}
		}
		else if(fadeTime <= 0){
			for(int i = 0; i < mats.Length; i++){
//				matCols.Add(mats[i].material.GetColor("_Shader"));
//				print (matCols[i]);
//				mats[i].material.SetColor("_Shader", SetColClear(mats[i].materials[0].color));
			}
			
			for(int j = 0; j < parts.Length; j ++){
				parts[j].startColor = Color.Lerp(parts[j].startColor,partCols[j],lerpSpeed);
				if(parts[j].startColor.a <= 15){
					parts[j].startColor = SetColClear(parts[j].startColor);
				}
			}
			Destroy (gameObject,destroyTime);
		}
	}

	public void FadeParticle(GameObject skillParticle){
		mats = skillParticle.transform.GetComponentsInChildren<MeshRenderer>();
		parts = skillParticle.transform.GetComponentsInChildren<ParticleSystem>();

		for(int i = 0; i < mats.Length; i++){
//			matCols[i] = mats[i].materials[0].color;
		}
		
		for(int j = 0; j < parts.Length; j ++){
			partCols.Add(parts[j].startColor);
			parts[j].startColor = SetColClear(parts[j].startColor);
		}

		lerpingCols = true;
	}
		
	public Color SetColClear(Color col){
		Color outCol;
		outCol = new Color(col.r,col.g,col.b,0);
		return outCol;
	}
}