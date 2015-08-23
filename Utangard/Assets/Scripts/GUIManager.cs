using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour {

	public static GUIManager inst;
	public GameObject GUICanvas;
	public Tooltip TooltipPanel;
	public Text CombatLog;
	public List<RawImage> playerAvatars;
	public List<RenderTexture> avatarRenderTextures; // 0 - Eir, 1 - Heimdal, 2 - Skadi, 3 -  Thor, 4 - Sam

	private Animator GUICanvasAnimator;

	void Start()
	{
		inst = this;
		GUICanvasAnimator = GUICanvas.GetComponent<Animator>();
	}


	public void LogCombatResult(string res)
	{
		CombatLog.text += (res + "\n");	
	}

	public void UpdatePlayerGUI(int currentPlayer){

		CombatLog.text = "";
		
		if(currentPlayer == 0){
			p1Turn = true;
			p2Turn = false;
		}
		else{
			p1Turn = false;
			p2Turn = true;
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

	private bool p1Turn{
		get{ return GUICanvasAnimator.GetBool("p1Turn"); }
		set{ GUICanvasAnimator.SetBool("p1Turn", value); }
	}

	private bool p2Turn{
		get{ return GUICanvasAnimator.GetBool("p2Turn"); }
		set{ GUICanvasAnimator.SetBool("p2Turn", value); }
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
