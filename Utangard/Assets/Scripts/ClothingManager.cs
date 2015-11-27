using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClothingManager : MonoBehaviour {
	//0 - Eir, 1 - Heimdall, 2 - Skadi, 3 - Thor
	public List<Material> spearSkins;
	public List<Material> axeSkins;
	public List<Material> shieldSkins;

	public void SetSkins(){
		for(int i = 0; i < Logic.Inst.Players.Length; i++){
			for(int j = 0; j < Logic.Inst.Players[i].army.Count; j++){

				Unit unit = Logic.Inst.Players[i].army[j];
				SkinnedMeshRenderer skin = null;

				if(unit.type != UnitType.Hero){
					if(unit.gameObject.transform.childCount > 0){
						Transform model = unit.gameObject.transform.GetChild(0);
						for(int x = 0; x < model.childCount; x++){
							Transform child = model.GetChild(x);
			
							if(child.name == "body"){
								skin = child.GetComponent<SkinnedMeshRenderer>();
							}
						}
					}

					switch(unit.Owner.hero.type){
					case HeroType.Eir:
						ChangeSkin(unit,0,skin);
						break;

					case HeroType.Heimdall:
						ChangeSkin(unit,1,skin);
						break;

					case HeroType.Skadi:
						ChangeSkin(unit,2,skin);
						break;

					case HeroType.Thor:
						ChangeSkin(unit,3,skin);
						break;

					default:
						print ("A " + unit.type +" has no hero?");
						break;
					}
				}
			}
		}
	}

	public void ChangeSkin(Unit unit, int skinNum, SkinnedMeshRenderer skin){
		for(int i = 0; i < skin.materials.Length; i++){
			if(unit.type == UnitType.Spearman){
				skin.materials[i].mainTexture = spearSkins[skinNum].mainTexture;
			}
			else if(unit.type == UnitType.Axemen){
				skin.materials[i].mainTexture = axeSkins[skinNum].mainTexture;
			}
			else if(unit.type == UnitType.Swordsmen){
				skin.materials[i].mainTexture = shieldSkins[skinNum].mainTexture;
			}
		}
	}
}