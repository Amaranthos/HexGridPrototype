using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour {

	public static GUIManager inst;
	public GameObject GUICanvas;
	public GUIAnimationController guiAnim;
	public Button EndTurnP1;
	public Button EndTurnP2;
	public Tooltip TooltipPanel;
	public Text CombatLog;
	public List<RawImage> playerAvatars;
	public List<RenderTexture> avatarRenderTextures; // 0 - Eir, 1 - Heimdal, 2 - Skadi, 3 -  Thor, 4 - Sam

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
		CombatLog.text += (res + "\n");	
	}

	public void UpdatePlayerGUI(int currentPlayer){

		CombatLog.text = "";
		guiAnim.ResetStates();
		if(currentPlayer == 0){
			guiAnim.SetP1Turn();
		}
		else{
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
		
	public void AssignTextures()
	{
		switch(Logic.Inst.Players[0].hero.type)
		{
		case HeroType.Eir:
			playerAvatars[0].texture = avatarRenderTextures[0];
			break;
		case HeroType.Heimdal:
			playerAvatars[0].texture = avatarRenderTextures[1];
			break;
		case HeroType.Skadi:
			playerAvatars[0].texture = avatarRenderTextures[2];
			break;
		case HeroType.Thor:
			playerAvatars[0].texture = avatarRenderTextures[3];
			break;
		case HeroType.Sam:
			playerAvatars[0].texture = avatarRenderTextures[4];
			break;
		}
		
		switch(Logic.Inst.Players[1].hero.type)
		{
		case HeroType.Eir:
			playerAvatars[1].texture = avatarRenderTextures[0];
			break;
		case HeroType.Heimdal:
			playerAvatars[1].texture = avatarRenderTextures[1];
			break;
		case HeroType.Skadi:
			playerAvatars[1].texture = avatarRenderTextures[2];
			break;
		case HeroType.Thor:
			playerAvatars[1].texture = avatarRenderTextures[3];
			break;
		case HeroType.Sam:
			playerAvatars[1].texture = avatarRenderTextures[4];
			break;
		}
	}
}
