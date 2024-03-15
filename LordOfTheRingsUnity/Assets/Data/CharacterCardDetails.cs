using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterCardDetails : CardDetails
{
    [Header("Character Card Details")]
    public bool isAvatar;
    public RacesEnum race;
    public SubRacesEnum subRace = SubRacesEnum.None;
    public List<CharacterClassEnum> classes;
    public List<CharacterAbilitiesEnum> abilities;
    public bool isImmovable;
    public bool isFemale;

    public short prowess;
    public short defence;
    public short influence;

    public FactionsEnum faction;
    private bool isLoaded;
    void Awake()
    {
        isLoaded = false;
    }

    public bool Initialize()
    {
        Resources requirements = new(0, 0, 0, 0, 0, 0, 0, 0);
        requirements.resources[ResourceType.FOOD] += prowess + defence;
        foreach (CharacterClassEnum c in classes)
        {
            switch (c)
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
        }

        isLoaded = base.Initialize(CardClass.Character, requirements);
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
    public List<string> GetClassesStrings()
    {
        return classes.Select(x => x.ToString()).ToList();
    }
    public List<string> GetRaceSubRaceStrings()
    {
        return new List<string>() { race.ToString() };
        // Subrace?
    }
    public List<string> GetAbilitiesStrings()
    {
        return abilities.Select(x => x.ToString()).ToList();
    }
    public List<CharacterAbilitiesEnum> GetAbilities()
    {
        return abilities;
    }

    public short GetMind() => (short) Math.Round((decimal)(prowess + defence) / 2);

    public int GetProwess()
    {
        return prowess;
    }

    public int GetDefence()
    {
        return defence;
    }

    public int GetInfluence()
    {
        return influence;
    }
}
