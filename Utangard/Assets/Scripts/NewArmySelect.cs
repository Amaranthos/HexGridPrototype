using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewArmySelect : MonoBehaviour {
	public enum SelectionStage{
		player1,
		player2,
	}
	
	public Army[] playerArmies = new Army[2];
	public GUI gui;
	public Materials materials;
	public GameObject altars;
	public int pickedThisTurn;
	public int pickPerTurn;
	public int pickPerArmy;
	public RuneStones unitStones;
	public SelectionStage whosPicking;
	bool rotating;
	int timesP1picked;
	int timesP2picked;
	
	void Start () {
		playerArmies[0].axes = 0;
		playerArmies[0].swords = 0;
		playerArmies[0].spears = 0;

		playerArmies[1].axes = 0;
		playerArmies[1].swords = 0;
		playerArmies[1].spears = 0;


		gui.subTitle.text = "Select " + pickPerTurn + " warriors";
		UpdateMaterials();
		AssignHeroBanners();

		// Turn P1 stuff off, turn on P2 stuff
		for(int i = 0; i < gui.P1descriptions.Length; i++)
		{
			gui.P1descriptions[i].SetActive(true);
		}
		for(int i = 0; i < gui.P2descriptions.Length; i++)
		{
			gui.P2descriptions[i].SetActive(false);
		}
	}



	public void Update(){
		gui.selectedNum.text = pickedThisTurn + "/" + pickPerTurn + " Selected";
		if(pickedThisTurn == pickPerTurn){
			gui.continueButton.interactable = true;
		}
		else{
			gui.continueButton.interactable = false;
		}

		if(whosPicking == SelectionStage.player1){
			if(altars.transform.rotation.eulerAngles.y != 0){
				if(altars.transform.rotation.eulerAngles.y > 0){
					altars.transform.Rotate(0,-12,0);
				}
				else{
					altars.transform.rotation = new Quaternion(0,0,0,0);
				}
			}
		}
		else{
			if(altars.transform.rotation.eulerAngles.y != 180){
				if(altars.transform.rotation.eulerAngles.y > 180){

				}
				else{
					altars.transform.Rotate(0,12,0);
				}
			}
		}

		gui.playerArmyCounts[0].text = "Player 1:   " + 
			(playerArmies[0].axes + playerArmies[0].spears + playerArmies[0].swords + 1) + "/10 units total";

		gui.playerArmyCounts[1].text = "Player 2:   " + 
			(playerArmies[1].axes + playerArmies[1].spears + playerArmies[1].swords + 1) + "/10 units total";

	}

	public void Continue(){
		if(whosPicking == SelectionStage.player1){
			whosPicking = SelectionStage.player2;
			timesP1picked += 1;
			pickedThisTurn = 0;
			if(timesP2picked == pickPerArmy-1){
				gui.title.text = "Player 2, Assemble your final warriors!";
			}
			else{
				gui.title.text = "Player 2, Assemble your warriors!";
			}
			// Turn P1 stuff off, turn on P2 stuff
			for(int i = 0; i < gui.P1descriptions.Length; i++)
			{
				gui.P1descriptions[i].SetActive(false);
			}
			for(int i = 0; i < gui.P2descriptions.Length; i++)
			{
				gui.P2descriptions[i].SetActive(true);
			}
		}
		else{
			whosPicking = SelectionStage.player1;
			timesP2picked += 1;
			pickedThisTurn = 0;
			if(timesP1picked == pickPerArmy-1){
				gui.title.text = "Player 1, Assemble your final warriors!";
			}
			else{
				gui.title.text = "Player 1, Assemble your warriors!";
			}
			// Turn P1 stuff on, turn off P2 stuff
			for(int i = 0; i < gui.P1descriptions.Length; i++)
			{
				gui.P1descriptions[i].SetActive(true);
			}
			for(int i = 0; i < gui.P2descriptions.Length; i++)
			{
				gui.P2descriptions[i].SetActive(false);
			}
		}
		if(timesP1picked == pickPerArmy && timesP2picked == pickPerArmy){
			pickPerTurn = 3;
			Logic.Inst.SetupGameWorld(playerArmies);
			gameObject.SetActive(false);
		}
	}

	public void AssignHeroBanners(){
		switch(Logic.Inst.Players[0].hero.type){
		case HeroType.Eir:
			unitStones.p1Banners[0].GetComponent<MeshRenderer>().material = materials.heroMaterials[0];
			unitStones.p1Banners[1].GetComponent<MeshRenderer>().material = materials.heroMaterials[0];
			unitStones.p1Banners[2].GetComponent<MeshRenderer>().material = materials.heroMaterials[0];
			break;
		case HeroType.Heimdall:
			unitStones.p1Banners[0].GetComponent<MeshRenderer>().material = materials.heroMaterials[1];
			unitStones.p1Banners[1].GetComponent<MeshRenderer>().material = materials.heroMaterials[1];
			unitStones.p1Banners[2].GetComponent<MeshRenderer>().material = materials.heroMaterials[1];
			break;
		case HeroType.Skadi:
			unitStones.p1Banners[0].GetComponent<MeshRenderer>().material = materials.heroMaterials[2];
			unitStones.p1Banners[1].GetComponent<MeshRenderer>().material = materials.heroMaterials[2];
			unitStones.p1Banners[2].GetComponent<MeshRenderer>().material = materials.heroMaterials[2];
			break;
		case HeroType.Thor:
			unitStones.p1Banners[0].GetComponent<MeshRenderer>().material = materials.heroMaterials[3];
			unitStones.p1Banners[1].GetComponent<MeshRenderer>().material = materials.heroMaterials[3];
			unitStones.p1Banners[2].GetComponent<MeshRenderer>().material = materials.heroMaterials[3];
			break;
		}
		switch(Logic.Inst.Players[1].hero.type){
		case HeroType.Eir:
			unitStones.p2Banners[0].GetComponent<MeshRenderer>().material = materials.heroMaterials[0];
			unitStones.p2Banners[1].GetComponent<MeshRenderer>().material = materials.heroMaterials[0];
			unitStones.p2Banners[2].GetComponent<MeshRenderer>().material = materials.heroMaterials[0];
			break;
		case HeroType.Heimdall:
			unitStones.p2Banners[0].GetComponent<MeshRenderer>().material = materials.heroMaterials[1];
			unitStones.p2Banners[1].GetComponent<MeshRenderer>().material = materials.heroMaterials[1];
			unitStones.p2Banners[2].GetComponent<MeshRenderer>().material = materials.heroMaterials[1];
			break;
		case HeroType.Skadi:
			unitStones.p2Banners[0].GetComponent<MeshRenderer>().material = materials.heroMaterials[2];
			unitStones.p2Banners[1].GetComponent<MeshRenderer>().material = materials.heroMaterials[2];
			unitStones.p2Banners[2].GetComponent<MeshRenderer>().material = materials.heroMaterials[2];
			break;
		case HeroType.Thor:
			unitStones.p2Banners[0].GetComponent<MeshRenderer>().material = materials.heroMaterials[3];
			unitStones.p2Banners[1].GetComponent<MeshRenderer>().material = materials.heroMaterials[3];
			unitStones.p2Banners[2].GetComponent<MeshRenderer>().material = materials.heroMaterials[3];
			break;
		}
	}

	public void UpdateMaterials(){
		unitStones.p1Stones[0].GetComponent<MeshRenderer>().material = materials.swordMaterials[playerArmies[0].swords];
		unitStones.p1Stones[1].GetComponent<MeshRenderer>().material = materials.axeMaterials[playerArmies[0].axes];
		unitStones.p1Stones[2].GetComponent<MeshRenderer>().material = materials.spearMaterials[playerArmies[0].spears];
		
		unitStones.p2Stones[0].GetComponent<MeshRenderer>().material = materials.swordMaterials[playerArmies[1].swords];
		unitStones.p2Stones[1].GetComponent<MeshRenderer>().material = materials.axeMaterials[playerArmies[1].axes];
		unitStones.p2Stones[2].GetComponent<MeshRenderer>().material = materials.spearMaterials[playerArmies[1].spears];
	}
	
	#region Add/Remove Units
	public void AddSword(){
		if(pickedThisTurn < pickPerTurn){
			if(whosPicking == SelectionStage.player1){
				playerArmies[0].swords += 1;
			}
			else{
				playerArmies[1].swords += 1;
			}
			pickedThisTurn += 1;
			UpdateMaterials();
		}
	}
	public void AddSpear(){
		if(pickedThisTurn < pickPerTurn){
			if(whosPicking == SelectionStage.player1){
				playerArmies[0].spears += 1;
			}
			else{
				playerArmies[1].spears += 1;
			}
			pickedThisTurn += 1;
			UpdateMaterials();
		}
	}
	public void AddAxe(){
		if(pickedThisTurn < pickPerTurn){
			if(whosPicking == SelectionStage.player1){
				playerArmies[0].axes += 1;
			}
			else{
				playerArmies[1].axes += 1;
			}
			pickedThisTurn += 1;
			UpdateMaterials();
		}
	}

	public void RemoveSword(){
		if(whosPicking == SelectionStage.player1){
			if(playerArmies[0].swords > 0){
				playerArmies[0].swords -= 1;
			}
		}
		else{
			if(playerArmies[1].swords > 0){
				playerArmies[1].swords -= 1;
			}
		}
		if(pickedThisTurn > 0){
			pickedThisTurn -= 1;
		}
		UpdateMaterials();
	}
	public void RemoveSpear(){
		if(whosPicking == SelectionStage.player1){
			if(playerArmies[0].spears > 0){
				playerArmies[0].spears -= 1;
			}
		}
		else{
			if(playerArmies[1].spears > 0){
				playerArmies[1].spears -= 1;
			}
		}
		if(pickedThisTurn > 0){
			pickedThisTurn -= 1;
		}
		UpdateMaterials();
	}
	public void RemoveAxe(){
		if(whosPicking == SelectionStage.player1){
			if(playerArmies[0].axes > 0){
				playerArmies[0].axes -= 1;
			}
		}
		else{
			if(playerArmies[1].axes > 0){
				playerArmies[1].axes -= 1;
			}
		}
		if(pickedThisTurn > 0){
			pickedThisTurn -= 1;
		}
		UpdateMaterials();
	}
	#endregion
}

[System.Serializable]
public struct RuneStones{
	public GameObject[] p1Stones;
	public GameObject[] p2Stones;
	public GameObject[] p1Banners;
	public GameObject[] p2Banners;
}

[System.Serializable]
public struct Materials{
	public Material[] spearMaterials;
	public Material[] swordMaterials;
	public Material[] axeMaterials;
	public Material[] heroMaterials;
}

[System.Serializable]
public struct GUI{
	public Text title;
	public Text subTitle;
	public Text selectedNum;
	public Button continueButton;
	public GameObject[] P1descriptions;
	public GameObject[] P2descriptions;
	public Text[] playerArmyCounts;
}

[System.Serializable]
public struct Army{
	public int spears;
	public int axes;
	public int swords;
}