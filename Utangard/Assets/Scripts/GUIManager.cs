using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour {

	[System.Serializable]public struct UnitPortraits{
		public string hero;
		public Sprite none;
		public Sprite axe;
		public Sprite spear;
		public Sprite sword;
	}

	[System.Serializable]public struct AbilityIcons{
		public string hero;
		public Sprite ability1;
		public Sprite ability2;
	}

	public static GUIManager inst;
	public GameObject GUICanvas;
	public GUIAnimationController guiAnim;
	public BattlePredictionUI battlePrediction;
	public Button EndTurnP1;
	public Button EndTurnP2;
//	public Tooltip TooltipPanel;
//	public Text CombatLog;
	public GameObject[] formationButtonPanels;
	public List<Image> playerAvatars;
	public List<Sprite> heroPortraits; // 0 - Eir, 1 - Heimdal, 2 - Skadi, 3 -  Thor, 4 - Sam
	public UnitPortraits[] unitPortraits;
	public AbilityIcons[] abilityIcons;


	//private Animator GUICanvasAnimator;
	private bool p1Turn;
	private bool p2Turn;

	void Start()
	{
		inst = this;
		//GUICanvasAnimator = GUICanvas.GetComponent<Animator>();
	}

	public void LogCombatResult(string res)
	{
		//CombatLog.text += (res + "\n");	
	}

	public void DisplayPredictions(Unit unitHovered){
		if(!battlePrediction.isOpen){
			battlePrediction.OpenPredictions(unitHovered);
		}
	}
	public void ClosePredictions(){
		battlePrediction.ClosePredictions();
	}

	public void UpdatePlayerGUI(int currentPlayer){

		//CombatLog.text = "";
		guiAnim.ResetStates();
		if(currentPlayer == 0){
			if(Logic.Inst.gamePhase == GamePhase.PlacingPhase){
				formationButtonPanels[0].SetActive(true);
			}
			else{
				formationButtonPanels[0].SetActive(false);
			}
			formationButtonPanels[1].SetActive(false);
			guiAnim.SetP1Turn();
		}
		else{
			if(Logic.Inst.gamePhase == GamePhase.PlacingPhase){
				formationButtonPanels[1].SetActive(true);
			}
			else{
				formationButtonPanels[1].SetActive(false);
			}
			formationButtonPanels[0].SetActive(false);
			guiAnim.SetP2Turn();
		}
	}

	public bool EndTurnButton{
		get{ 
			if(Logic.Inst.CurrentPlayerNum == 0){
			return EndTurnP1.interactable; 
			}
			else{
				return EndTurnP2.interactable;
			}
		}
		set{ 
			if(Logic.Inst.CurrentPlayerNum == 0){
			EndTurnP1.interactable = value; 
			}
			else{
				EndTurnP2.interactable = value;
			}
		}
	}

	public void TogglePanel(GUIPanel panel)
	{
		if (panel.IsOpen)
		{
			// Close Panel
			panel.IsOpen = false;
		}
		else
		{
			panel.IsOpen = true;
		}
	}
}
