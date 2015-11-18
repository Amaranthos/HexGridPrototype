using UnityEngine;
using System.Collections;

public class Altar : MonoBehaviour {
	[SerializeField]
	private Player owner = null;

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
			Transform model = gameObject.transform.FindChild("Alter_nobase");
			for(int j = 0; j < model.transform.childCount; j++){
				GameObject child = model.GetChild(j).gameObject;
				if(child.name == "Plane001" || child.name == "Plane002" || child.name == "Plane003"){
					if(!child.GetComponent<SkinnedMeshRenderer>()){
						MeshRenderer meshRend = child.GetComponent<MeshRenderer>();
						meshRend.material = new Material(meshRend.material);
						meshRend.material.color = owner.playerColour;
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
