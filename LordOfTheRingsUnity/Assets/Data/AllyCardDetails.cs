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

    private bool isLoaded;

    void Awake()
    {
        isLoaded = false;
    }

    public bool Initialize()
    {
        Resources requirements = new(0, 0, 0, 0, 0, 0, 0, 0);
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
        isLoaded = Initialize(CardClass.Ally, requirements);
        return isLoaded;
    }

    public bool IsLoaded()
    {
        return isLoaded;
    }

    void Update()
    {
        if (!isLoaded)
            Initialize();
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
