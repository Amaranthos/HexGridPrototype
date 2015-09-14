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
	Dodge
}

public enum PassiveType {
	None,
	Global,
    Persitent,
    OneShot
}

public enum TargetType {
	All,
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