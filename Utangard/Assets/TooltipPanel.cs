using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TooltipPanel : MonoBehaviour {

	public TooltipType type;
	public Text[] textFields;
	public Animator anim;

	public void ExpandTip(){
		Expand = true;
	}
	public void CloseTip(){

	}


	bool Expand{
		get{ return anim.GetBool("Expand"); }
		set{ anim.SetBool("Expand", value); }
	}

}
