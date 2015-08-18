using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class GUIManager : MonoBehaviour {

	public static GUIManager inst;
	public GameObject GUICanvas;
	public Tooltip TooltipPanel;

	private Animator GUICanvasAnimator;

	void Start()
	{
		inst = this;
		GUICanvasAnimator = GUICanvas.GetComponent<Animator>();
	}

	public void UpdatePlayerGUI(int currentPlayer){

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
}
