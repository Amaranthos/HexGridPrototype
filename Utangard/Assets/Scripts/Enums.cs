public enum UnitType {
	Spearman,
	Axemen,
	Swordsmen,
	Hero,
	None
}

public enum HeroType {
	Eir,
	Heimdal,
	Skadi,
	Thor,
	Sam
}

public enum EffectType {
	Health,
	Attack,
	Defense,
	Move,
	Range,
	Damage,
	Hit,
	Dodge,
	Position
}

public enum PassiveType {
	None,
	Buff,
    PersitentAoE,
    OneShotAoE
}

public enum AimType {
	All,
	Single,
	SelfAoE,
	TargetAoE
}

public enum TargetType {
	Single,
	AoE
}

public enum GamePhase {
	HeroSelectPhase,
	ArmySelectPhase,
	PlacingPhase,
	CombatPhase,
	TargetPhase,
	FinishedPhase
}

public enum AbilityStage {
	GetUnit,
	GetLocation,
	Done
}

public enum AbilityType {
	Buff,
	Teleport
}

public enum BuffType {
	Stat,
	Adjacent,
	HexTerrain
}

public enum AdjacencyType{
	Friends,
	Enemies,
	Both
}

public enum TerrainType{
	Hills,
	Plains,
	Mountains
}

public enum BiomeType{
	Grass,
	Snow,
	Forest
}

public enum Music {
	BloodOath
}

public enum SFX {
	Attack_Success,
	Attack_Fail,
	Can_Attack,
	Hero_Laugh,
	Rune_Roll,
	Scroll,
	Scroll_Up,
	Select,
	Select_2,
	Unit_Click,
	Unit_CantMoveThere,
	Unit_Death,
	Unit_Move
}