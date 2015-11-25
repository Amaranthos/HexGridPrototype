using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Unit : MonoBehaviour {

	public UnitType type;
	private CubeIndex index;

	public int movePoints;
	public int maxHitpoints;
	public int attack;
	public int defense;
	public int cost;

	[Range(0,100)]
	public int hitChance;
	[Range(0,100)]
	public int dodgeChance;
	public int attackRange;

	private int attackModifer = 0;
	private int defenseModifer = 0;
	private int hitModifier = 0;
	private int dodgeModifier = 0;
	private int currentHP;
	private int currentMP;

	private bool canMove = true;
	public bool dead = false;

	private Player owner;

	public List<Buff> currentBuffs = new List<Buff>();	//Every effect this unit is currently under
	private Queue<Tile> currentPath;

	private List<Tile> highlighted = new List<Tile>();

	public Animator unitAnim;

	public List<TextSpawn> buffsToSpawn = new List<TextSpawn>();
	private float buffDelay = 1f;

	private SpriteRenderer ringSprite;

	private void Start() {
		currentHP = maxHitpoints;
		currentMP = movePoints;
		unitAnim = GetComponentInChildren<Animator>();
		ringSprite = gameObject.transform.FindChild("Unit select ring").GetComponent<SpriteRenderer>();
		ringSprite.color = new Color(0,0,0,0);

		if(Logic.Inst.CurrentPlayerNum != -1 && type == UnitType.Hero){
			unitAnim.SetBool("InGame",true);
		}
	}

	public void MoveTowardsTile(Tile tile) {
		// Logic.Inst.Audio.PlaySFX(SFX.Unit_Move);
		CubeIndex tempIndex = index;

		Logic.Inst.Grid.TileAt(index).OccupyingUnit = null;

		if (Logic.Inst.gamePhase == GamePhase.CombatPhase){
			currentPath = new Queue<Tile>(Logic.Inst.Path.GetPath(Logic.Inst.Grid.TileAt(index), tile));
			currentPath = new Queue<Tile>(currentPath.Reverse());
			StartCoroutine(Move());
		}		
		else {
			transform.position = tile.transform.position;
		}

		index = tile.Index;
		tile.OccupyingUnit = this;
		//Handles adjacency buffs
		AdjacencyCheck(tempIndex);

		//Handles persistent passives
		PersistentAoECheck();

		ClearHighlightedTiles();
	}

	private IEnumerator Move(){
		if(unitAnim){
			ChangeAnim(1);
		}

		while(currentPath.Count != 0){
			if(unitAnim){
				ChangeAnim(1);
			}

			Tile tile = currentPath.Dequeue();
			Quaternion dir = Quaternion.LookRotation(tile.transform.position - transform.position);
			while(transform.rotation != dir){
				transform.rotation = Quaternion.RotateTowards(transform.rotation, dir, Time.deltaTime * 2f * 360f/Mathf.PI);
				yield return new WaitForEndOfFrame();
			}

			while(transform.position != tile.transform.position){
				transform.position = Vector3.MoveTowards(transform.position, tile.transform.position, Time.deltaTime * 2f);
				yield return new WaitForEndOfFrame();	
			}
			transform.position = tile.transform.position;
			currentMP -= tile.MoveCost;

			if(currentMP == 0){
				ringSprite.color = new Color(1,0,0,0.5f);
			}

			if(unitAnim){
				ChangeAnim(0);
			}

			yield return new WaitForSeconds(0.25f);
		}
		if(Logic.Inst.CurrentPlayer == Owner && !Logic.Inst.SelectedUnit){
			Logic.Inst.UnitSelected(this);
			Logic.Inst.HighlightMoveRange(this);
		}

		if(unitAnim){
			ChangeAnim(0);
		}

		yield return null;
	}

	public void UnitSelected(){
		if(canMove){
			FloodFill flood = new FloodFill();

			List<Tile> ret = flood.Flood(Logic.Inst.Grid.TileAt(index), currentMP);

			for(int i = 0; i < ret.Count; i++){
				if(ret[i].OccupyingUnit){
					if(ret[i].OccupyingUnit.Owner == Owner){
						ret[i].LineColour(Color.grey);
						ret[i].LineWidth(0.1f);
						highlighted.Add(ret[i]);
					}
					else if(InAttackRange(ret[i].OccupyingUnit)) {
						ret[i].LineColour(Color.red);
						ret[i].LineWidth(0.1f);
						highlighted.Add(ret[i]);
					}
				}
				else {
					ret[i].LineColour(Color.green);
			 		ret[i].LineWidth(0.1f);
					highlighted.Add(ret[i]);
				}
			}

			ret = Logic.Inst.Grid.TilesInRange(index, attackRange);

			for(int i = 0; i < ret.Count; i++){
				if(ret[i].OccupyingUnit && ret[i].OccupyingUnit.Owner != Owner){
					ret[i].LineColour(Color.red);
					ret[i].LineWidth(0.1f);
					highlighted.Add(ret[i]);
				}
			}

			Logic.Inst.Grid.TileAt(index).LineColour(Color.cyan);
			Logic.Inst.Grid.TileAt(index).LineWidth(0.1f);
			highlighted.Add(Logic.Inst.Grid.TileAt(index));	
		}

		ringSprite.color = new Color(1,1,0,0.5f);
	}

	public void ClearHighlightedTiles() {
		for(int i = 0; i < highlighted.Count; i++){
			highlighted[i].LineColour(Color.black);
			highlighted[i].LineWidth(0.03f);			
		}

		highlighted.Clear();
	}

	public void UnitKilled() {
		ChangeAnim(3);

		dead = true;
		owner.RemoveUnit(this);

		if(owner.army.Count <= 0){
			Logic.Inst.PlayerEliminated(owner);
		}

		if(type == UnitType.Hero && owner.hero.passive.passive == PassiveType.PersitentAoE){
			for(int i = 0; i < owner.army.Count; i++){
				RemovePassiveBuff(owner.army[i], true);
			}
		}

		// Logic.Inst.Audio.PlaySFX(SFX.Unit_Death);

		ringSprite.color = new Color(0,0,0,0);
	}

	public void DestroyUnit(){
		Debug.Log(type + " was killed");
		Destroy(this.gameObject);
	}

	public void UnitSacrificed() {
		owner.Faith += Logic.Inst.faithPtsPerSacrifice;
		owner.RemoveUnit(this);
		Debug.Log(type + " was sacrificed");
		DestroyImmediate(this.gameObject);
		// Logic.Inst.Audio.PlaySFX(SFX.Unit_Death);
	}

	public void OnMouseOver(){
		if(Logic.Inst.SelectedUnit != null){
			if(Logic.Inst.SelectedUnit != this){
				if(owner != Logic.Inst.CurrentPlayer){
					GUIManager.inst.DisplayPredictions(this);
				}
			}
		}
	}

	public void OnMouseExit(){
		GUIManager.inst.ClosePredictions();
	}

	public bool InAttackRange(Unit unit) {
		return Logic.Inst.Grid.TilesInRange(index, attackRange).Contains(Logic.Inst.Grid.TileAt(unit.Index));
	}

	public bool InMoveRange(Tile tile) {
		List<Tile> ret = Logic.Inst.Grid.TilesInRange(index, movePoints);

		if(currentMP > 0)
			ret.RemoveAll(item => {
				int pathCost = Logic.Inst.Path.PathCost(Logic.Inst.Path.GetPath(Logic.Inst.Grid.TileAt(index), item));
				return (pathCost > currentMP || pathCost <= 0);
			});
		else
			ret.Clear();
		return ret.Contains(tile);
	}

	public void OnTurnStart(){
		// GameObject tempText;
		List<int> finishedBuffs = new List<int>();

		currentMP = movePoints;

		for(int i = 0; i < currentBuffs.Count; i++){
			currentBuffs[i].duration--;
			if(currentBuffs[i].duration == 0 &&  !currentBuffs[i].permanent){				//If the effect is done
				currentBuffs[i].ChangeValue(this,false);	//Alter this units relative stat. False indicates that the effect is being removed.
//				buffsToSpawn.Add(new TextSpawn(bff,this,false));
//				SpawnBuffText(bff,this,false);
				finishedBuffs.Add(currentBuffs[i].ID);
				Logic.Inst.gameObject.GetComponent<ParticleManager>().RemoveParticle(currentBuffs[i].skillID);
			}
		}

//		SpawnBuffText(buffsToSpawn);

		RemoveBuffs(this,finishedBuffs);

		CalculateModifiers(true);

		ringSprite.color = new Color(0,1,1,0.5f);
	}

	public void OnTurnEnd(){
		ringSprite.color  = new Color(0,0,0,0);
	}

	public void AddBuff(Buff bff){
		Buff nEft = null;
		bool newBuff = true;
		// GameObject tempText;

		for(int i = 0; i < currentBuffs.Count; i++){
			if(currentBuffs[i].ID == bff.ID){
				newBuff = false;
				currentBuffs[i].duration = bff.duration;
			}
		}

		if(!bff.oneShot && newBuff){
			nEft = new Buff(bff.ID,bff.skillID,bff.buffType,bff.duration,bff.effectType,bff.strength,bff.wrath,bff.targetType,bff.permanent,bff.procced,bff.debuff,bff.oneShot,bff.adjType,bff.adjUnits,bff.timesProcced,bff.isBio,bff.terType,bff.bioType);
			currentBuffs.Add(nEft);
		}

		if(bff.buffType == BuffType.Stat && newBuff){
			bff.ChangeValue(this,true);
		}

		if (newBuff) {
//			SpawnBuffText (nEft,this,true);
			buffsToSpawn.Add(new TextSpawn(nEft,this,true));
		}
	}

	public void AdjacencyCheck(CubeIndex prevIndex){
		for(int i = 0; i < currentBuffs.Count; i++){
			List<Tile> inRange = new List<Tile>();
			List<Tile> inPrevRange = new List<Tile>();
			inRange = Logic.Inst.Grid.TilesInRange(index,1);
			inPrevRange = Logic.Inst.Grid.TilesInRange(prevIndex,1);
			
			if(currentBuffs[i].buffType == BuffType.Adjacent){
				CalculateModifiers(false);
			}
			
			for(int j = 0; j < inRange.Count; j++){
				if(inRange[j].OccupyingUnit){
					for(int k = 0; k < inRange[j].OccupyingUnit.currentBuffs.Count; k++){
						if(inRange[j].OccupyingUnit.currentBuffs[k].buffType == BuffType.Adjacent && AdjProc(inRange[j].OccupyingUnit,inRange[j].OccupyingUnit.currentBuffs[k])){
							inRange[j].OccupyingUnit.CalculateModifiers(false);
						}
					}
				}
			}

			for(int x = 0; x < inPrevRange.Count; x++){
				if(inPrevRange[x].OccupyingUnit){
					for(int y = 0; y < inPrevRange[x].OccupyingUnit.currentBuffs.Count; y++){
						if(inPrevRange[x].OccupyingUnit.currentBuffs[y].buffType == BuffType.Adjacent && AdjProc(inPrevRange[x].OccupyingUnit,inPrevRange[x].OccupyingUnit.currentBuffs[y])){
							inPrevRange[x].OccupyingUnit.CalculateModifiers(false);
						}
					}
				}
			}
		}
	}

	public void CalculateModifiers(bool turnStart){
		attackModifer = 0;
		defenseModifer = 0;
		hitModifier = 0;
		dodgeModifier = 0;
		
		CalcAdjacency(turnStart);

		for(int i = 0; i < currentBuffs.Count; i++){
			if((currentBuffs[i].duration > 0 || currentBuffs[i].permanent) && currentBuffs[i].buffType == BuffType.Stat){
				currentBuffs[i].ChangeValue(this,true);
				if(!currentBuffs[i].procced){
					buffsToSpawn.Add(new TextSpawn(currentBuffs[i],this,true));
				}
			}
		}

		StartCoroutine("SpawnBuffText",buffsToSpawn);
	}

	public void CalcAdjacency(bool turnStart){
		List<Tile> inRange = new List<Tile>();
		int proced = 0;
		bool makeText = true;

		for(int i = 0; i < currentBuffs.Count; i++){
			Buff buff = currentBuffs[i];
			Buff tempBuff = new Buff(buff.ID,buff.skillID,buff.buffType,buff.duration,buff.effectType,0,buff.wrath,buff.targetType,buff.permanent,buff.procced,buff.debuff,buff.oneShot,buff.adjType,buff.adjUnits,buff.timesProcced,buff.isBio,buff.terType,buff.bioType);

			proced = 0;
			if(buff.buffType == BuffType.Adjacent){
				inRange = Logic.Inst.Grid.TilesInRange(index,1);
				for(int j = 0; j < inRange.Count; j++){
					if(inRange[j].OccupyingUnit && AdjProc(inRange[j].OccupyingUnit,buff)){
						buff.ChangeValue(this,true);
						proced++;
					}
				}

				if(proced > 0){
					buff.ChangeValue(this,false); //To account for the fact that the unit will count itself.
					proced--;
				}

				if(proced <= 0){
					if(buff.timesProcced <= 0){
						makeText = false;
					}
					else{
						tempBuff.strength = buff.strength * buff.timesProcced;
						if(!turnStart){
							buffsToSpawn.Add(new TextSpawn(tempBuff,this,false));
						}
					}
				}
				else{
//					if(proced < buff.timesProcced){
//						//I would use this to set the buff text to be relative to the total change, rather than the total strength.
//					}
//					else{
						tempBuff.procced = false;
						tempBuff.strength = buff.strength * proced;
						if(!turnStart){
							buffsToSpawn.Add(new TextSpawn(tempBuff,this,true));
						}
//					}
				}

				buff.timesProcced = proced;
			}
		}

		if(makeText){
			StartCoroutine("SpawnBuffText",buffsToSpawn);
		}
	}

	private bool AdjProc(Unit unit, Buff buff){
		switch(buff.adjType){
		case AdjacencyType.Friends:
			if(buff.adjUnits.Contains(unit.type) && unit.owner == owner){
				return true;
			}
			break;
			
		case AdjacencyType.Enemies:
			if(buff.adjUnits.Contains(unit.type) && unit.owner != owner){
				return true;
			}
			break;
			
		case AdjacencyType.Both:
			if(buff.adjUnits.Contains(unit.type)){
				return true;
			}
			break;
		}

		return false;
	}

	public void PersistentAoECheck(){
		List<Tile> inRange = new List<Tile>();

		inRange = Logic.Inst.Grid.TilesInRange(index,owner.hero.passive.AoERange);

		if(type == UnitType.Hero && owner.hero.passive.passive == PassiveType.PersitentAoE){
			for(int i = 0; i < owner.army.Count; i++){
				if(!inRange.Contains(Logic.Inst.Grid.TileAt(owner.army[i].index))){
					RemovePassiveBuff(owner.army[i],true);
					buffDelay = 0;
				}
			}
			
			owner.hero.passive.ApplyBuffAoE(index);
		}
		else if(owner.hero.passive.passive == PassiveType.PersitentAoE){
			if(inRange.Contains(Logic.Inst.Grid.TileAt(owner.hero.gameObject.GetComponent<Unit>().index))){
				owner.hero.passive.ApplyBuffSingle(index);
			}
			else{
				RemovePassiveBuff(this, true);
			}
		}
	}

	private void RemovePassiveBuff(Unit unit, bool makeText){
		int buffToRemove = -1;

		for(int i = 0; i < unit.currentBuffs.Count; i++){
			if(buffToRemove < 0){
				for(int j = 0; j < unit.owner.hero.passive.buffs.Count; j++){
					if(unit.currentBuffs[i].ID == unit.owner.hero.passive.buffs[j].ID && unit.owner.hero.passive.affected.Contains(unit.type)){
						buffToRemove = unit.currentBuffs.IndexOf(unit.currentBuffs[i]);
					}
				}
			}
		}
		if(buffToRemove > -1){
//			if(makeText){
				buffsToSpawn.Add(new TextSpawn(unit.currentBuffs[buffToRemove],unit,false));
//			}
			unit.currentBuffs[buffToRemove].ChangeValue(unit,false);
			unit.currentBuffs.RemoveAt(buffToRemove);
			buffToRemove = -1;
			Logic.Inst.gameObject.GetComponent<ParticleManager>().RemoveParticle(unit.owner.hero.passive.ID);
		}

		StartCoroutine("SpawnBuffText",buffsToSpawn);
	}

	private void RemoveBuffs(Unit unit, List<int> buffID){
		int buffToRemove = -1;

		for(int i = 0; i < buffID.Count; i++){
			for(int j = 0; j < unit.currentBuffs.Count; j++){
				if(unit.currentBuffs[j].ID == buffID[i]){
					buffToRemove = unit.currentBuffs.IndexOf(unit.currentBuffs[j]);
				}
			}
			if(buffToRemove > -1){
				buffsToSpawn.Add(new TextSpawn(unit.currentBuffs[buffToRemove],unit,false));
				unit.currentBuffs.RemoveAt(buffToRemove);
				buffToRemove = -1;
				print(buffsToSpawn[i].buff.ID);
			}
		}

		StartCoroutine("SpawnBuffText",buffsToSpawn);
	}
	
	public IEnumerator SpawnBuffText(List<TextSpawn> buffList){
		GameObject tempText = null;
		int multiplier = 1;
		string operatorString;

		// print("SPAWNING BUFFS");

		if(buffList.Count < 2){
			buffDelay = 0f;
		}

		for(int i = 0; i < buffList.Count; i++){
//			if (buffList[i].buff.buffType != BuffType.Adjacent) {
				if (buffList[i].add) {
					multiplier = 1;
					operatorString = "+ ";
				} 
				else {
					multiplier = -1;
					operatorString = "";
				}

				if (buffList[i].buff.effectType != EffectType.Damage && buffList[i].buff.effectType != EffectType.Health) {
					if((!buffList[i].add && buffList[i].buff.procced) || (buffList[i].add && !buffList[i].buff.procced)){
						tempText = MonoBehaviour.Instantiate (Logic.Inst.buffText, (buffList[i].unit.gameObject.transform.position + Vector3.up * Logic.Inst.offsetDist), Quaternion.identity) as GameObject;
						tempText.GetComponent<TextMesh> ().text = operatorString + (buffList[i].buff.strength * multiplier) + " " + buffList[i].buff.effectType;
					}
				}

				if (!buffList[i].add) {
					buffList[i].buff.procced = false;
				} 
				else {
					buffList[i].buff.procced = true;
				}
//			}

			yield return new WaitForSeconds(buffDelay);
		}

		buffsToSpawn.Clear();
		buffDelay = 1f;
	}

	public void ChangeAnim(int animState){
		if(unitAnim){
			unitAnim.SetInteger("State",animState);
		}
	}

	public void CaptureAltar(){
		Altar altar = Logic.Inst.GetAltar(index);
		
		if (altar){
			if(altar.Owner != Owner){
				canMove = false;
				currentMP = 0;
				altar.PlayerCaptureAltar(owner);
				Logic.Inst.StartCoroutine("ShowAltarText");
				ClearHighlightedTiles();
				// UnitSelected();
			}
		}
	}

	public void OnAttack(){
		ringSprite.color = new Color(0.5f,0.5f,0.5f,0.5f);
	}

	public void OnDeselect(){
		if(owner == Logic.Inst.CurrentPlayer){
			if(currentMP > 0 && canMove){
				ringSprite.color = new Color(0,1,1,0.5f);
			}
			else if(canMove){
				ringSprite.color = new Color(1,0,0,0.5f);
			}
			else{
				ringSprite.color = new Color(0.5f,0.5f,0.5f,0.5f);
			}
		}
		else{
			ringSprite.color = new Color(0,0,0,0);
		}
	}
		
	#region Getters and Setters
	public CubeIndex Index {
		get { return index; }
		set { index = value; }
	}

	public int CurrentHitpoints {
		get { return currentHP; }
		set { currentHP = value; }
	}

	public int AttackModifier {
		get { return attackModifer; }
		set { attackModifer = value; }
	}

	public int DefenseModifier {
		get { return defenseModifer; }
		set { defenseModifer= value; }
	}

	public int HitModifier {
		get { return hitModifier; }
		set { hitModifier = value; }
	}

	public int DodgeModifier {
		get { return dodgeModifier; }
		set { dodgeModifier = value; }
	}

	public int TotalAttack {
		get { return attack + attackModifer; }
	}

	public int TotalDefense {
		get { return defense + defenseModifer; }
	}

	public int TotalHitChance {
		get { return hitChance + hitModifier; }
	}

	public int TotalDodgeChance {
		get { return dodgeChance + dodgeModifier; }
	}

	public Player Owner {
		get { return owner; }
		set { owner = value; }
	}

	public bool CanMove {
		get { return canMove; }
		set { canMove = value; }
	}

	public int CurrentMovePoints {
		get { return currentMP; }
		set { currentMP = value; }
	}
	#endregion
}