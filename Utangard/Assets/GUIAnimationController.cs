using UnityEngine;
using System.Collections;

public class GUIAnimationController : MonoBehaviour {

//	private bool p1Turn;
//	private bool p2Turn;
//	private Animator anim;
	public Animator P1Anim;
	public Animator P2Anim;

	void Update(){
		CheckConditions();
	}

	public void CheckConditions(){
		if(Logic.Inst.gamePhase == GamePhase.PlacingPhase){
			if(Logic.Inst.SelectedUnit != null){
				if(Logic.Inst.CurrentPlayerNum == 0){
					P1Unit = true;
				}
				P2Unit = true;
			}
			else{
				if(Logic.Inst.CurrentPlayerNum == 0){
					P1Unit = false;
				}
				else{
					P2Unit = false;
				}
			}
		}
		else if(Logic.Inst.gamePhase == GamePhase.CombatPhase){
			// Player 1's Turn?
			if(Logic.Inst.CurrentPlayerNum == 0)
			{
				// Unit is Selected?
				if(Logic.Inst.SelectedUnit != null)
				{
					// Hero Selected?
					if(Logic.Inst.SelectedUnit.type == UnitType.Hero)
					{
						// Hero on Altar?
						if(Logic.Inst.GetAltar(Logic.Inst.SelectedUnit.Index) != null)
						{
							P1HeroShrine = true;
							P1Unit = false;
							P1Hero = false;
							P1Shrine = false;
						}
						else
						{
							P1HeroShrine = false;
							P1Unit = false;
							P1Hero = true;
							P1Shrine = false;
						}
					}
					// Unit Selected
					else
					{
						// Unit is on Altar?
						if(Logic.Inst.GetAltar(Logic.Inst.SelectedUnit.Index) != null)
						{
							P1HeroShrine = false;
							P1Unit = false;
							P1Hero = false;
							P1Shrine = true;
						}
						//Unit is not on Altar
						else
						{
							P1HeroShrine = false;
							P1Unit = true;
							P1Hero = false;
							P1Shrine = false;
						}
					}
				}
				// Unit Not Selected
				else{
					P1Unit = false;
					P1Hero = false;
					P1Shrine = false;
					P1HeroShrine = false;
				}
			}
			// Player 2's Turn
			// Unit is Selected?
			if(Logic.Inst.SelectedUnit != null)
			{
				// Hero Selected?
				if(Logic.Inst.SelectedUnit.type == UnitType.Hero)
				{
					// Hero on Altar?
					if(Logic.Inst.GetAltar(Logic.Inst.SelectedUnit.Index) != null)
					{
						P2HeroShrine = true;
						P2Unit = false;
						P2Hero = false;
						P2Shrine = false;
					}
					else
					{
						P2HeroShrine = false;
						P2Unit = false;
						P2Hero = true;
						P2Shrine = false;
					}
				}
				// Unit Selected
				else
				{
					// Unit is on Altar?
					if(Logic.Inst.GetAltar(Logic.Inst.SelectedUnit.Index) != null)
					{
						P2HeroShrine = false;
						P2Unit = false;
						P2Hero = false;
						P2Shrine = true;
					}
					//Unit is not on Altar
					else
					{
						P2HeroShrine = false;
						P2Unit = true;
						P2Hero = false;
						P2Shrine = false;
					}
				}
			}
			// Unit Not Selected
			else{
				P2Unit = false;
				P2Hero = false;
				P2Shrine = false;
				P2HeroShrine = false;
			}
		}
	}

	public void ResetStates(){
		P1Open = false;
		P1Unit = false;
		P1Hero = false;
		P1Shrine = false;
		P1HeroShrine = false;

		P2Open = false;
		P2Unit = false;
		P2Hero = false;
		P2Shrine = false;
		P2HeroShrine = false;
	}

	public void SetP1Turn(){
		print ("SetP1Turn()");
		P1Open = true;
		P2Open = false;
	}

	public void SetP2Turn(){
		print ("SetP1Turn()");
		P2Open = true;
		P1Open = false;
	}

	#region GetSets

	public bool P1Open{
		get{ return P1Anim.GetBool("P1Open"); }
		set{ P1Anim.SetBool("P1Open", value); }
	}

	public bool P2Open{
		get{ return P2Anim.GetBool("P2Open"); }
		set{ P2Anim.SetBool("P2Open", value); }
	}

	public bool P1Unit{
		get{ return P1Anim.GetBool("P1Unit"); }
		set{ P1Anim.SetBool("P1Unit", value); }
	}
	
	public bool P2Unit{
		get{ return P2Anim.GetBool("P2Unit"); }
		set{ P2Anim.SetBool("P2Unit", value); }
	}

	public bool P1Hero{
		get{ return P1Anim.GetBool("P1Abilities"); }
		set{ P1Anim.SetBool("P1Abilities", value); }
	}
	
	public bool P2Hero{
		get{ return P2Anim.GetBool("P2Abilities"); }
		set{ P2Anim.SetBool("P2Abilities", value); }
	}
	public bool P1Shrine{
		get{ return P1Anim.GetBool("P1Shrine"); }
		set{ P1Anim.SetBool("P1Shrine", value); }
	}
	
	public bool P2Shrine{
		get{ return P2Anim.GetBool("P2Shrine"); }
		set{ P2Anim.SetBool("P2Shrine", value); }
	}
	public bool P1HeroShrine{
		get{ return P1Anim.GetBool("P1ShrineAbilities"); }
		set{ P1Anim.SetBool("P1ShrineAbilities", value); }
	}
	
	public bool P2HeroShrine{
		get{ return P2Anim.GetBool("P2ShrineAbilities"); }
		set{ P2Anim.SetBool("P2ShrineAbilities", value); }
	}
	#endregion
}
