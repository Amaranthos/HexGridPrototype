using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShrineButton : MonoBehaviour {

	public int playerNum;
	public GameObject shrineButton;
	public Text toolTip;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		CheckShrineCondition();
	}

	public void CheckShrineCondition(){
		if(Logic.Inst.CurrentPlayerNum == playerNum){
			if(playerNum == 0){
				if(GUIManager.inst.guiAnim.P1Shrine || GUIManager.inst.guiAnim.P1HeroShrine)
				{
					if(Logic.Inst.GetAltar(Logic.Inst.SelectedUnit.Index).Owner != Logic.Inst.CurrentPlayer){
						shrineButton.GetComponentInChildren<Text>().text = "Capture";
						toolTip.text = 
						"Capture the Altar in the name of <size=17><b>"+ Logic.Inst.CurrentPlayer.hero.name +"</b></size> \n" +
						"<color=lime>+ " + Logic.Inst.faithPtsPerAltar + " Faith per turn</color>";
					}
					else{
						shrineButton.GetComponentInChildren<Text>().text = "Sacrifice";
						toolTip.text =
						"Sacrifice this unit to <size=17><b>"+ Logic.Inst.CurrentPlayer.hero.name +"</b></size> \n" +
						"<color=lime>+ " + Logic.Inst.faithPtsPerSacrifice +" Faith</color>";
					}
				}
			}
			else if(playerNum == 1){
				if(GUIManager.inst.guiAnim.P2Shrine || GUIManager.inst.guiAnim.P2HeroShrine)
				{
					if(Logic.Inst.GetAltar(Logic.Inst.SelectedUnit.Index).Owner != Logic.Inst.CurrentPlayer){
						shrineButton.GetComponentInChildren<Text>().text = "Capture";
						toolTip.text = 
						"Capture the Altar in the name of <size=17><b>"+ Logic.Inst.CurrentPlayer.hero.name +"</b></size> \n " +
						"<color=lime>+ " + Logic.Inst.faithPtsPerAltar + " Faith per turn</color>";
					}
					else{
						shrineButton.GetComponentInChildren<Text>().text = "Sacrifice";
						toolTip.text = 
						"Sacrifice this unit to <size=17><b>"+ Logic.Inst.CurrentPlayer.hero.name +"</b></size> \n" +
						"<color=lime>+ " + Logic.Inst.faithPtsPerSacrifice +" Faith</color>";
					}
				}
			}
		}

	}
	
	public void ShrineButtonFunc(){
		if(playerNum == 0){
			if(GUIManager.inst.guiAnim.P1Shrine || GUIManager.inst.guiAnim.P1HeroShrine){
				if(Logic.Inst.GetAltar(Logic.Inst.SelectedUnit.Index) != null){
					if(Logic.Inst.GetAltar(Logic.Inst.SelectedUnit.Index).Owner != Logic.Inst.CurrentPlayer){
						Logic.Inst.CaptureAlar();
					}
					else if(Logic.Inst.GetAltar(Logic.Inst.SelectedUnit.Index).Owner == Logic.Inst.CurrentPlayer){
						Logic.Inst.SacrificeUnit();
					}
				}
			}
		}
		else if(playerNum == 1){
			if(GUIManager.inst.guiAnim.P2Shrine || GUIManager.inst.guiAnim.P2HeroShrine){
				if(Logic.Inst.GetAltar(Logic.Inst.SelectedUnit.Index) != null){
					if(Logic.Inst.GetAltar(Logic.Inst.SelectedUnit.Index).Owner != Logic.Inst.CurrentPlayer){
						Logic.Inst.CaptureAlar();
					}
					else if(Logic.Inst.GetAltar(Logic.Inst.SelectedUnit.Index).Owner == Logic.Inst.CurrentPlayer){
						Logic.Inst.SacrificeUnit();
					}
				}
			}
		}
	}
}
