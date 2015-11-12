using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewArmySelect : MonoBehaviour {

	[System.Serializable]public struct RuneStones{
		public GameObject[] p1Stones;
		public GameObject[] p2Stones;
	}

	[System.Serializable]public struct Materials{
		public Material[] spearMaterials;
		public Material[] swordMaterials;
		public Material[] axeMaterials;
	}

	[System.Serializable]public struct GUI{
		public Text title;
		public Text subTitle;
		public Text selectedNum;
		public Button continueButton;
		public GameObject[] P1descriptions;
		public GameObject[] P2descriptions;
	}

	[System.Serializable]public struct Army{
		public int spears;
		public int axes;
		public int swords;
	}

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

		UpdateMaterials();
	}

	public void UpdateMaterials(){
		unitStones.p1Stones[0].GetComponent<MeshRenderer>().material = materials.swordMaterials[playerArmies[0].swords];
		unitStones.p1Stones[1].GetComponent<MeshRenderer>().material = materials.axeMaterials[playerArmies[0].axes];
		unitStones.p1Stones[2].GetComponent<MeshRenderer>().material = materials.spearMaterials[playerArmies[0].spears];

		unitStones.p2Stones[0].GetComponent<MeshRenderer>().material = materials.swordMaterials[playerArmies[1].swords];
		unitStones.p2Stones[1].GetComponent<MeshRenderer>().material = materials.axeMaterials[playerArmies[1].axes];
		unitStones.p2Stones[2].GetComponent<MeshRenderer>().material = materials.spearMaterials[playerArmies[1].spears];
	}

	public void Update(){
		if(whosPicking == SelectionStage.player1){
			if(altars.transform.rotation.eulerAngles.y != 0){
				altars.transform.Rotate(0,5,0);
			}
		}
		else{
			if(altars.transform.rotation.eulerAngles.y != 180){
				if(altars.transform.rotation.eulerAngles.y > 180){
					Vector3 temp = new Vector3(45,0,0);
					altars.transform.rotation = Quaternion.Euler(temp);
				}
				else{
					altars.transform.Rotate(0,5,0);
				}
			}
		}
	}

	public void Continue(){
		if(whosPicking == SelectionStage.player1){
			whosPicking = SelectionStage.player2;
		}
		else{
			whosPicking = SelectionStage.player1;
		}
	}

	private float ClampAngle(float angle, float min, float max){
		if (angle<90 || angle>270){       // if angle in the critic region...
			if (angle>180) 
				angle -= 360;  // convert all angles to -180..+180
			if (max>180) 
				max -= 360;
			if (min>180) 
				min -= 360;
		}    
		angle = Mathf.Clamp(angle, min, max);
		if (angle<0) 
			angle += 360;  // if angle negative, convert to 0..360
		
		return angle;
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
