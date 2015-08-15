using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class GUIManager : MonoBehaviour {

	public static GUIManager inst;
	public Tooltip TooltipPanel;

	void Start()
	{
		inst = this;
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
