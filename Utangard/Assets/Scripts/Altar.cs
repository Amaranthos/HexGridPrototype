using UnityEngine;
using System.Collections.Generic;

public class Altar : MonoBehaviour {
	[SerializeField]
	private Player owner = null;

	public List<BannerMat> banners = new List<BannerMat> ();

	public CubeIndex Index { get; set; }

	public void PlayerCaptureAltar(Player player) {
		if (owner){
			owner.capturedAltars.Remove(this);
			if(player.capturedAltars.Count > 3){
				Logic.Inst.Music.ChangeBase(MusicBaseState.NearWin);
			}
			else {
				Logic.Inst.Music.ChangeBase(MusicBaseState.Battle);	
			}
		}
		owner = player;
		player.capturedAltars.Add(this);


		if(gameObject.transform.childCount > 0){
			Transform model = gameObject.transform.FindChild("Altar");
			for(int j = 0; j < model.transform.childCount; j++){
				GameObject child = model.GetChild(j).gameObject;
				if(child.name == "LeftBanner001" || child.name == "MainBanner001" || child.name == "RightBanner001"){
					if(!child.GetComponent<SkinnedMeshRenderer>()){
						MeshRenderer meshRend = child.GetComponent<MeshRenderer>();

//						BannerMat mat = banners.Find(item => item.hero == player.hero.type).mat;

						meshRend.material = banners.Find(item => item.hero == player.hero.type).mat;
//						meshRend.material = new Material(meshRend.material);
//						meshRend.material.color = owner.playerColour;
					}
				}
			}

			for(int i = 0; i < transform.childCount; i++){
				GameObject child = transform.GetChild(i).gameObject;
				if(child.name == "Motes" || child.name == "Glow"){
					child.GetComponent<ParticleSystem>().startColor = owner.playerColour;
				}
			}
		}
	}

	public Player Owner{
		get {return owner;}
	}
}

[System.Serializable]
public struct BannerMat{
	public HeroType hero;
	public Material mat;
}