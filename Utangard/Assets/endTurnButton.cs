using UnityEngine;
using System.Collections;

public class endTurnButton : MonoBehaviour {

    public int playerNum;
    UnityEngine.UI.Button button;

	// Use this for initialization
	void Start () {
        button = gameObject.GetComponent<UnityEngine.UI.Button>();
	}
	
	// Update is called once per frame
	void Update () {
        if (playerNum == Logic.Inst.currentPlayer)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
	}
}
