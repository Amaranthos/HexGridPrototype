using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeploymentText : MonoBehaviour {

    public Text playerTurn;

    void Update() {
        
        if (Logic.Inst.CurrentPlayerNum == 0)
        {
            playerTurn.text = "Player 1's Turn";
        }
        else 
        {
            playerTurn.text = "Player 2's Turn";
        }
    }

}
