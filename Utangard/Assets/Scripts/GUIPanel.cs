using UnityEngine;
using System.Collections;

public class GUIPanel : MonoBehaviour {

	private Animator m_Animator;
	
	public bool IsOpen
	{
		get{ return m_Animator.GetBool("IsOpen"); }
		set{ m_Animator.SetBool("IsOpen", value); }
	}

	void Awake()
	{
		m_Animator = GetComponent<Animator> ();
	}
}
