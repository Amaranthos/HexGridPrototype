using UnityEngine;
using System.Collections;

public class WrathModeTextAnim : MonoBehaviour {

	Animator anim;

	// Use this for initialization
	void Start () {
	
		anim = GetComponent<Animator>();
	}

	public bool On{
		get{ return anim.GetBool("Active");}
		set{ 
			anim.SetBool("Active", value);
			if(!anim.GetBool("Active")){
				GetComponent<UnityEngine.UI.Text>().enabled = false;
			}
			else{
				GetComponent<UnityEngine.UI.Text>().enabled = true;
			}
		}
	}

}
