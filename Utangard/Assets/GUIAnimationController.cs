using UnityEngine;
using System.Collections;

public class GUIAnimationController : MonoBehaviour {

//	private bool p1Turn;
//	private bool p2Turn;
	private Animator anim;

	// Use this for initialization
	void Awake () {
		anim = GetComponent<Animator>();
		// print (anim);
	}

	void Update(){
		if(Logic.Inst.gamePhase == GamePhase.CombatPhase){
			if(Logic.Inst.SelectedUnit != null){
				if(Logic.Inst.SelectedUnit == Logic.Inst.CurrentPlayer.hero.hero){
					if(Logic.Inst.CurrentPlayerNum == 0){
						P1HeroSelected = true;
					}
					else{
						P2HeroSelected = true;
					}
				}
				else{
					P1HeroSelected = false;
					P2HeroSelected = false;
				}
			}
			else{
				P1HeroSelected = false;
				P2HeroSelected = false;
			}
		}
	}

	public void SetP1Turn(){
		p1Turn = true;
		p2Turn = false;
	}

	public void SetP2Turn(){
		p1Turn = false;
		p2Turn = true;
	}

	public bool P1HeroSelected{
		get{ return anim.GetBool("P1Hero"); }
		set{ anim.SetBool("P1Hero", value); }
	}

	public bool UnitOnAltar{
		get { return anim.GetBool("P1Shrine"); }
		set { anim.SetBool("P1Shrine", value); }
	}
	
	public bool P2HeroSelected{
		get{ return anim.GetBool("P2Hero"); }
		set{ anim.SetBool("P2Hero", value); }
	}

	private bool p1Turn{
		get{ return anim.GetBool("P1Base"); }
		set{ anim.SetBool("P1Base", value); }
	}
	private bool p2Turn{
		get{ return anim.GetBool("P2Base"); }
		set{ anim.SetBool("P2Base", value); }
	}


}
