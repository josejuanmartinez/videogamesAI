using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllyCardDetails : CardDetails
{
    [SerializeField]
    private CharacterClassEnum allyClass;
    [SerializeField]
    private RacesEnum race;
    [SerializeField]
    private SubRacesEnum subRace;
    [SerializeField]
    private List<CharacterAbilitiesEnum> abilities;

    [SerializeField]
    private short prowess;
    [SerializeField]
    private short defence;

    void Awake()
    {
        Resources requirements= new (0, 0, 0, 0, 0, 0, 0, 0);
        requirements.resources[ResourceType.FOOD] += prowess + defence;
        switch (allyClass)
        {
            case CharacterClassEnum.Warrior:
                requirements.resources[ResourceType.METAL] += prowess + defence;
                break;
            case CharacterClassEnum.Scout:
                requirements.resources[ResourceType.LEATHER] += prowess + defence;
                break;
            case CharacterClassEnum.Sage:
                requirements.resources[ResourceType.CLOTHES] += prowess + defence;
                break;
            case CharacterClassEnum.Diplomat:
                requirements.resources[ResourceType.CLOTHES] += prowess + defence;
                break;
            case CharacterClassEnum.Agent:
                requirements.resources[ResourceType.LEATHER] += prowess + defence;
                break;
        }
        base.Initialize(CardClass.Ally, requirements);
    }

    public void Initialize()
    {
        Awake();
    }
    public short GetProwess() {
        return prowess;
    }
    public short GetDefence()
    {
        return defence;
    }
    public int GetMind()
    {
        return (int)Math.Round((decimal)(prowess + defence) / 2);
    }

    public RacesEnum GetRace()
    {
        return race;
    }

    public SubRacesEnum GetSubRace()
    {
        return subRace;
    }

    public CharacterClassEnum GetClass()
    { 
        return allyClass;
    }
    public List<CharacterAbilitiesEnum> GetAbilities()
    {
        return abilities;
    }

    public List<string> GetAbilitiesStrings()
    {
        return abilities.Select(x => x.ToString()).ToList();
    }

}
