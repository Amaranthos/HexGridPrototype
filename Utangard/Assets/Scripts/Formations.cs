using UnityEngine;
using System.Collections.Generic;

public class Formations : MonoBehaviour {

	public UnitFormations form;

	private List<Tile> map;
	private List<CubeIndex> p1Indexes = new List<CubeIndex>();
	private List<CubeIndex> p2Indexes = new List<CubeIndex>();

	private int startColumn = -1;

	public void InitField(Army[] armies) {
		// Setup placement fields
		List<Tile> tiles = map.FindAll(item=>item.Index.x <= startColumn);

		for(int i = 0; i < tiles.Count; i++){
			p1Indexes.Add (tiles[i].Index);
		}

		for(int i = 0; i < tiles.Count; i++){
			// Player 2's placement field is rotated pi radians
			p2Indexes.Add (new CubeIndex(-p1Indexes[i].x, -p1Indexes[i].y, -p1Indexes[i].z));
		}

		Logic.Inst.Players[0].placementField = p1Indexes;
		Logic.Inst.Players[1].placementField = p2Indexes;

		// Get the appropriate formation and spawn units
		for(int i = 0; i < Logic.Inst.Players.Length; i++){
			Logic.Inst.Players[i].units = armies[i];
			var formation = GetFormation(i, armies[i]);

			for(int j = 0; j < formation.Count; j++){
				if(formation[j].First != UnitType.Hero){
					Logic.Inst.Players[i].SpawnUnit(formation[j].First, Logic.Inst.Grid.TileAt(formation[j].Second), i);
				}
				else {
					Logic.Inst.Players[i].SpawnHero(Logic.Inst.Grid.TileAt(formation[j].Second), i);	
				}
			}			
		}
	}

	public void Reform(int player, Army army){
		Logic.Inst.Players[player].ClearFormation();

		var formation = GetFormation(player, army);

		for(int j = 0; j < formation.Count; j++){
			if(formation[j].First != UnitType.Hero){
				Logic.Inst.Players[player].SpawnUnit(formation[j].First, Logic.Inst.Grid.TileAt(formation[j].Second), player);
			}
			else {
				Logic.Inst.Players[player].SpawnHero(Logic.Inst.Grid.TileAt(formation[j].Second), player);
			}
		}

		gameObject.GetComponent<ClothingManager>().SetSkins();
	}

	public List<Pair<UnitType, CubeIndex>> GetFormation(int player, Army troops){
		var formation = FormPositions(troops);

		// Rotate formation for player 2
		if(player == 1){
			for(int i = 0; i < formation.Count; i++){
				formation[i].Second = new CubeIndex(-formation[i].Second.x, -formation[i].Second.y, -formation[i].Second.z);
			}
		}
		return formation;
	}

	public List<Pair<UnitType, CubeIndex>> FormPositions(Army troops) {
		switch(form){
			case UnitFormations.Aggressive:
				return AggressiveFormation(troops);

			case UnitFormations.Defensive:
				return DefensiveFormation(troops);

			case UnitFormations.SkirmishAgg:
				return SkirmishAggFormation(troops);

			case UnitFormations.SkirmishDef:
				return SkirmishDefFormation(troops);

			case UnitFormations.Glorious:
				return GloriousFormation(troops);
		}
		return null;
	}

	// 	                                                                                          
	// 	  ,ad8888ba,      ad88    ad88                                 88                         
	// 	 d8"'    `"8b    d8"     d8"                                   ""                         
	// 	d8'        `8b   88      88                                                               
	// 	88          88 MM88MMM MM88MMM ,adPPYba, 8b,dPPYba,  ,adPPYba, 88 8b       d8  ,adPPYba,  
	// 	88          88   88      88   a8P_____88 88P'   `"8a I8[    "" 88 `8b     d8' a8P_____88  
	// 	Y8,        ,8P   88      88   8PP""""""" 88       88  `"Y8ba,  88  `8b   d8'  8PP"""""""  
	// 	 Y8a.    .a8P    88      88   "8b,   ,aa 88       88 aa    ]8I 88   `8b,d8'   "8b,   ,aa  
	// 	  `"Y8888Y"'     88      88    `"Ybbd8"' 88       88 `"YbbdP"' 88     "8"      `"Ybbd8"'  
	// 	                                                                                          
	// 	                                                                                          
		
	public List<Pair<UnitType, CubeIndex>> AggressiveFormation(Army troops){
		var formation = new List<Pair<UnitType, CubeIndex>>();

		int x = startColumn; // Start column
		int z = 0;
		int c = 0;

		for(int i = 0; i < troops.axes; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Axemen, new CubeIndex(x,z)));
		}

		for(int i = 0; i < troops.spears; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Spearman, new CubeIndex(x,z)));
		}

		x -= 1;
		z = Mathf.Abs(x)/2;
		c = 1;

		formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Hero, new CubeIndex(x,z)));

		for(int i = 0; i < troops.swords; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Swordsmen, new CubeIndex(x,z)));
		}
		return formation;
	}

	// 	                                                                                            
	// 	88888888ba,                 ad88                                 88                         
	// 	88      `"8b               d8"                                   ""                         
	// 	88        `8b              88                                                               
	// 	88         88  ,adPPYba, MM88MMM ,adPPYba, 8b,dPPYba,  ,adPPYba, 88 8b       d8  ,adPPYba,  
	// 	88         88 a8P_____88   88   a8P_____88 88P'   `"8a I8[    "" 88 `8b     d8' a8P_____88  
	// 	88         8P 8PP"""""""   88   8PP""""""" 88       88  `"Y8ba,  88  `8b   d8'  8PP"""""""  
	// 	88      .a8P  "8b,   ,aa   88   "8b,   ,aa 88       88 aa    ]8I 88   `8b,d8'   "8b,   ,aa  
	// 	88888888Y"'    `"Ybbd8"'   88    `"Ybbd8"' 88       88 `"YbbdP"' 88     "8"      `"Ybbd8"'  
	// 	                                                                                            
	// 	                                                                                            

	public List<Pair<UnitType, CubeIndex>> DefensiveFormation(Army troops){
		var formation = new List<Pair<UnitType, CubeIndex>>();

		int x = startColumn; // Start column
		int z = 0;
		int c = 0;

		for(int i = 0; i < troops.swords; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Swordsmen, new CubeIndex(x,z)));
		}

		x -= 1;
		z = Mathf.Abs(x)/2;
		c = 0;

		for(int i = 0; i < troops.spears; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Spearman, new CubeIndex(x,z)));
		}

		for(int i = 0; i < troops.axes; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Axemen, new CubeIndex(x,z)));
		}

		x -= 1;
		z = Mathf.Abs(x)/2;
		c = 1;

		formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Hero, new CubeIndex(x,z)));

		return formation;
	}

	// 	                                                                                                                
	// 	 ad88888ba  88        88                               88           88            ,ad8888ba,      ad88    ad88  
	// 	d8"     "8b 88        ""                               ""           88           d8"'    `"8b    d8"     d8"    
	// 	Y8,         88                                                      88          d8'        `8b   88      88     
	// 	`Y8aaaaa,   88   ,d8  88 8b,dPPYba, 88,dPYba,,adPYba,  88 ,adPPYba, 88,dPPYba,  88          88 MM88MMM MM88MMM  
	// 	  `"""""8b, 88 ,a8"   88 88P'   "Y8 88P'   "88"    "8a 88 I8[    "" 88P'    "8a 88          88   88      88     
	// 	        `8b 8888[     88 88         88      88      88 88  `"Y8ba,  88       88 Y8,        ,8P   88      88     
	// 	Y8a     a8P 88`"Yba,  88 88         88      88      88 88 aa    ]8I 88       88  Y8a.    .a8P    88      88     
	// 	 "Y88888P"  88   `Y8a 88 88         88      88      88 88 `"YbbdP"' 88       88   `"Y8888Y"'     88      88     
	// 	                                                                                                                
	// 	                                                                                                                

	public List<Pair<UnitType, CubeIndex>> SkirmishAggFormation(Army troops){
		var formation = new List<Pair<UnitType, CubeIndex>>();

		int x = startColumn; // Start column
		int z = 0;
		int c = 0;

		for(int i = 0; i < troops.spears; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Spearman, new CubeIndex(x,z)));
		}

		for(int i = 0; i < troops.axes; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Axemen, new CubeIndex(x,z)));
		}		

		x -= 1;
		z = Mathf.Abs(x)/2;
		c = 1;

		formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Hero, new CubeIndex(x,z)));

		for(int i = 0; i < troops.swords; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Swordsmen, new CubeIndex(x,z)));
		}
		return formation;
	}

	// 	                                                                                                                  
	// 	 ad88888ba  88        88                               88           88          88888888ba,                 ad88  
	// 	d8"     "8b 88        ""                               ""           88          88      `"8b               d8"    
	// 	Y8,         88                                                      88          88        `8b              88     
	// 	`Y8aaaaa,   88   ,d8  88 8b,dPPYba, 88,dPYba,,adPYba,  88 ,adPPYba, 88,dPPYba,  88         88  ,adPPYba, MM88MMM  
	// 	  `"""""8b, 88 ,a8"   88 88P'   "Y8 88P'   "88"    "8a 88 I8[    "" 88P'    "8a 88         88 a8P_____88   88     
	// 	        `8b 8888[     88 88         88      88      88 88  `"Y8ba,  88       88 88         8P 8PP"""""""   88     
	// 	Y8a     a8P 88`"Yba,  88 88         88      88      88 88 aa    ]8I 88       88 88      .a8P  "8b,   ,aa   88     
	// 	 "Y88888P"  88   `Y8a 88 88         88      88      88 88 `"YbbdP"' 88       88 88888888Y"'    `"Ybbd8"'   88     
	// 	                                                                                                                  
	// 	                                                                                                                  

	public List<Pair<UnitType, CubeIndex>> SkirmishDefFormation(Army troops){
		var formation = new List<Pair<UnitType, CubeIndex>>();

		int x = startColumn; // Start column
		int z = 0;
		int c = 0;

		for(int i = 0; i < troops.spears; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Spearman, new CubeIndex(x,z)));
		}

		for(int i = 0; i < troops.swords; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Swordsmen, new CubeIndex(x,z)));
		}		

		x -= 1;
		z = Mathf.Abs(x)/2;
		c = 1;

		formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Hero, new CubeIndex(x,z)));

		for(int i = 0; i < troops.axes; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Axemen, new CubeIndex(x,z)));
		}		
		return formation;
	}

	// 	                                                                              
	// 	  ,ad8888ba,  88                        88                                    
	// 	 d8"'    `"8b 88                        ""                                    
	// 	d8'           88                                                              
	// 	88            88  ,adPPYba,  8b,dPPYba, 88  ,adPPYba,  88       88 ,adPPYba,  
	// 	88      88888 88 a8"     "8a 88P'   "Y8 88 a8"     "8a 88       88 I8[    ""  
	// 	Y8,        88 88 8b       d8 88         88 8b       d8 88       88  `"Y8ba,   
	// 	 Y8a.    .a88 88 "8a,   ,a8" 88         88 "8a,   ,a8" "8a,   ,a88 aa    ]8I  
	// 	  `"Y88888P"  88  `"YbbdP"'  88         88  `"YbbdP"'   `"YbbdP'Y8 `"YbbdP"'  
	// 	                                                                              
	// 	                                                                              

	public List<Pair<UnitType, CubeIndex>> GloriousFormation(Army troops){
		var formation = new List<Pair<UnitType, CubeIndex>>();

		int x = startColumn; // Start column
		int z = 0;
		int c = 1;

		formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Hero, new CubeIndex(x,z)));
		
		for(int i = 0; i < troops.axes; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Axemen, new CubeIndex(x,z)));
		}

		x -= 1;
		z = Mathf.Abs(x)/2;
		c = 0;	

		for(int i = 0; i < troops.spears; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Spearman, new CubeIndex(x,z)));
		}

		for(int i = 0; i < troops.swords; i++){
			// Set the new position
			z += ((c&1) == 0)? -c : c;
			c++;

			// Position out of bounds
			if(z > 5 || z < -x - 5){
				c = 1;
				x -= 1;
				z = Mathf.Abs(x)/2;
			}

			// Add to formation
			formation.Add(new Pair<UnitType,CubeIndex>(UnitType.Swordsmen, new CubeIndex(x,z)));
		}
		return formation;
	}	

	public List<Tile> Map {
		set {map = value;}
	}
}