﻿using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadGame(){
		Application.LoadLevel("Test");
	}

	public void QuitGame(){
		if (!Application.isEditor) {
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}
	}
}
