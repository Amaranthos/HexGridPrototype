using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TooltipPanel : MonoBehaviour {

	public TooltipType type;
	public GameObject[] thingsToTurnOff;
	// Unit: 0 = Name, 1 = HP, 2 = moves, 3 = atk, 4 = def, 5 = hit, 6 = dodge
	public Text[] textFields;
	public Unit hoverUnit;
    public Tile hoverTile;
	public bool Open;
	Animator anim;

	void Start(){
		anim = GetComponent<Animator>();
	}

	void Update(){
		if(Open){
            Behave();
		}
	}

	void Behave(){
		switch(type)
		{
		case TooltipType.unit:
			if(hoverUnit != null){
				textFields[0].text = hoverUnit.name;
				textFields[1].text = hoverUnit.CurrentHitpoints.ToString();
				textFields[2].text = hoverUnit.CurrentMovePoints.ToString();
				textFields[3].text = hoverUnit.TotalAttack.ToString();
				textFields[4].text = hoverUnit.TotalDefense.ToString();
				textFields[5].text = hoverUnit.TotalHitChance.ToString();
				textFields[6].text = hoverUnit.TotalDodgeChance.ToString();
			}
			break;
        case TooltipType.terrain:
            if (hoverTile != null)
            {
                GetTerrainName();
                GetTerrainInfo();
            }
            break;
		}
	}

    void GetTerrainInfo()
    {
        if (hoverTile.IsPassable)
        {
            textFields[1].text = "Move Cost: " + hoverTile.MoveCost + "\n";
        }
        else 
        {
            textFields[1].text = "IMPASSABLE" + "\n";
            return;
        }

        if (hoverTile.hasAltar)
        {
            textFields[1].text += "<color=yellow>Altar " + "(" + Logic.Inst.GetAltar(hoverTile.index).Owner.hero.type + ")</color>" + "\n";
        }
        textFields[1].text += "<size=15>";
        switch (hoverTile.Biome)
        { 
            case BiomeType.Forest:
                textFields[1].text += "<color=lime>+ 10 " + "Dodge Chance</color>" + "\n";
                textFields[1].text += "<color=red>- 5 " + "Hit Chance</color>" + "\n";
                break;
            case BiomeType.Snow:
                textFields[1].text += "<color=lime>+ 10" + "Hit Chance </color>" + "\n";
                textFields[1].text += "<color=red>- 5" + " Dodge Chance</color>" + "\n";
                break;
        }
        switch (hoverTile.Terrain)
        { 
            case TerrainType.Hills:
                textFields[1].text += "<color=red>- 3" + " Attack</color>" + "\n";
                textFields[1].text += "<color=lime>+ 7" + " Defence</color>" + "\n";
                break;
        }
        textFields[1].text += "</size>";
    }

    void GetTerrainName()
    {
        switch (hoverTile.Biome)
        {
            case BiomeType.Forest:
                textFields[0].text = "Forested ";
                break;
            case BiomeType.Grass:
                textFields[0].text = "Grassy ";
                break;
            case BiomeType.Snow:
                textFields[0].text = "Snowy ";
                break;
        }
        if (hoverTile.hasAltar)
        {
            textFields[0].text += "Altar";
           
        }
        else
        {
            switch (hoverTile.Terrain)
            {
                case TerrainType.Hills:
                    textFields[0].text += "Hills";
                    break;
                case TerrainType.Mountains:
                    textFields[0].text += "Mountain";
                    break;
                case TerrainType.Plains:
                    textFields[0].text += "Plains";
                    break;
            }
        }
    }

	public void TurnOn(){
		Open = true;
        if (type == TooltipType.unit)
        {
            thingsToTurnOff[0].GetComponent<Image>().enabled = true;
            for (int i = 1; i < thingsToTurnOff.Length; i++)
            {
                thingsToTurnOff[i].SetActive(true);
            }
        }
	}
	public void TurnOff(){
		Open = false;
		thingsToTurnOff[0].GetComponent<Image>().enabled = false;
        for (int i = 1; i < thingsToTurnOff.Length; i++)
        {
            thingsToTurnOff[i].SetActive(false);
        }
	}

	public void ExpandTip(){
		Expand = true;
        if (type == TooltipType.terrain)
        {
            thingsToTurnOff[0].GetComponent<Image>().enabled = true;
            thingsToTurnOff[1].SetActive(true);
        }
	}

    public void CloseTip(){
		Expand = false;
	}

	bool Expand{
		get{ return anim.GetBool("Expand"); }
		set{ anim.SetBool("Expand", value); }
	}

    public void TerrainTipInfoOn()
    {
        thingsToTurnOff[2].SetActive(true);
        thingsToTurnOff[3].SetActive(true);
    }

}
