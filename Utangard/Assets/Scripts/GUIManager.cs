using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour {

	[System.Serializable]public struct VictoryTexts{
		public Text whoWinning;
		public Text timeLeft;
	}

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

	[Header("GUI Elements")]
	public MasterTooltip tooltips;
	public BattlePredictionUI battlePrediction;
	public GameObject deploymentText;
	public Button EndTurnP1;
	public Button EndTurnP2;
//	public Tooltip TooltipPanel;
//	public Text CombatLog;
	public VictoryTexts winText;
	public GameObject[] formationButtonPanels;
	public WrathModeTextAnim[] wrathText;
	public List<Image> playerAvatars;

	[Header("Sprites")]
	public List<Sprite> heroPortraits; // 0 - Eir, 1 - Heimdall, 2 - Skadi, 3 -  Thor, 4 - Sam
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

	public void ExpandTooltip(TooltipType type)
	{
		tooltips.ExpandTooltip(type);
	}
	public void OpenToolip(TooltipType type, Unit hoverUnit)
	{
		tooltips.ActivateTooltip(type, hoverUnit);
	}
	public void CloseTooltip()
	{
		tooltips.DeactivateTooltips();
	}

	public void SetWrathTextOn(int player){
		wrathText[player].On = true;
	}

	public void SetWrathTextOff(int player){
		wrathText[1].On = false;
		wrathText[0].On = false;
	}

	public void ReturnToMain()
	{
		Logic.Inst.Music.ChangeBase(MusicBaseState.Title);
		Application.LoadLevel("Main Menu");
	}

	public void SetPlayerWinning(int turnsRemain)
	{
		winText.whoWinning.text = Logic.Inst.CurrentWinner.hero.hero.name + " Controls all the Altars!";
		winText.timeLeft.text = turnsRemain + " turns until " + Logic.Inst.CurrentWinner.hero.hero.name + " Victory";
	}

	public void DisablePlayerWinning()
	{
		winText.whoWinning.text = "";
		winText.timeLeft.text = "";
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
				deploymentText.SetActive(true);
			}
			else{
				formationButtonPanels[0].SetActive(false);
				deploymentText.SetActive(false);
			}
			formationButtonPanels[1].SetActive(false);
			guiAnim.SetP1Turn();
		}
		else{
			if(Logic.Inst.gamePhase == GamePhase.PlacingPhase){
				formationButtonPanels[1].SetActive(true);
				deploymentText.SetActive(true);
			}
			else{
				formationButtonPanels[1].SetActive(false);
				deploymentText.SetActive(false);
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
