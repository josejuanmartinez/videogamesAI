using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HazardCreatureCardDetails: CardDetails
{
    [Header("HazardCreatureCardDetails")]
    public RacesEnum race;

    public short prowess;
    public short defence;

    public List<CardTypesEnum> cardTypes;
    public List<HazardAbilitiesEnum> hazardAbilities;

    private bool isLoaded = false;
    
    public bool Initialize()
    {
        Resources requirements = new(0, 0, 0, 0, 0, 0, 0, 0);
        requirements.resources[ResourceType.FOOD] += (prowess + defence) * 5;
        switch (race)
        {
            case RacesEnum.Man:
                requirements.resources[ResourceType.METAL] += prowess + defence;
                requirements.resources[ResourceType.LEATHER] += prowess + defence;
                break;
            case RacesEnum.Dwarf:
                requirements.resources[ResourceType.METAL] += prowess + defence;
                requirements.resources[ResourceType.WOOD] += prowess + defence;
                break;
            case RacesEnum.Elf:
                requirements.resources[ResourceType.LEATHER] += prowess + defence;
                requirements.resources[ResourceType.WOOD] += prowess + defence;
                break;
            case RacesEnum.Hobbit:
                requirements.resources[ResourceType.LEATHER] += prowess + defence;
                requirements.resources[ResourceType.CLOTHES] += prowess + defence;
                break;
            case RacesEnum.Dunadan:
                requirements.resources[ResourceType.METAL] += prowess + defence;
                requirements.resources[ResourceType.CLOTHES] += prowess + defence;
                break;
            case RacesEnum.Ringwraith:
                requirements.resources[ResourceType.METAL] += prowess + defence;
                requirements.resources[ResourceType.CLOTHES] += prowess + defence;
                break;
            case RacesEnum.Orc:
                requirements.resources[ResourceType.METAL] += prowess + defence;
                requirements.resources[ResourceType.WOOD] += prowess + defence;
                break;
            case RacesEnum.Troll:
                requirements.resources[ResourceType.METAL] += prowess + defence;
                requirements.resources[ResourceType.WOOD] += prowess + defence;
                break;
            case RacesEnum.Wizard:
                requirements.resources[ResourceType.CLOTHES] += prowess + defence;
                requirements.resources[ResourceType.GEMS] += prowess + defence;
                break;
            case RacesEnum.FallenWizard:
                requirements.resources[ResourceType.CLOTHES] += prowess + defence;
                requirements.resources[ResourceType.GEMS] += prowess + defence;
                break;
            case RacesEnum.Balrog:
                requirements.resources[ResourceType.METAL] += prowess + defence;
                requirements.resources[ResourceType.GOLD] += prowess + defence;
                break;
            case RacesEnum.Wolf:
                requirements.resources[ResourceType.FOOD] += prowess + defence;
                requirements.resources[ResourceType.MOUNTS] += prowess + defence;
                break;
            case RacesEnum.Animal:
                requirements.resources[ResourceType.FOOD] += prowess + defence;
                requirements.resources[ResourceType.MOUNTS] += prowess + defence;
                break;
            case RacesEnum.Trap:
                requirements.resources[ResourceType.WOOD] += prowess + defence;
                requirements.resources[ResourceType.METAL] += prowess + defence;
                break;
            case RacesEnum.Undead:
                requirements.resources[ResourceType.METAL] += prowess + defence;
                requirements.resources[ResourceType.GEMS] += prowess + defence;
                break;
            case RacesEnum.Spider:
                requirements.resources[ResourceType.FOOD] += prowess + defence;
                requirements.resources[ResourceType.MOUNTS] += prowess + defence;
                break;
            case RacesEnum.Plant:
                requirements.resources[ResourceType.FOOD] += prowess + defence;
                requirements.resources[ResourceType.FOOD] += prowess + defence;
                break;
            case RacesEnum.Bear:
                requirements.resources[ResourceType.FOOD] += prowess + defence;
                requirements.resources[ResourceType.MOUNTS] += prowess + defence;
                break;
            case RacesEnum.Dragon:
                requirements.resources[ResourceType.GOLD] += prowess + defence;
                requirements.resources[ResourceType.GEMS] += prowess + defence;
                break;
            case RacesEnum.Maia:
                requirements.resources[ResourceType.CLOTHES] += prowess + defence;
                requirements.resources[ResourceType.GEMS] += prowess + defence;
                break;
            case RacesEnum.Weather:
                requirements.resources[ResourceType.CLOTHES] += prowess + defence;
                requirements.resources[ResourceType.GEMS] += prowess + defence;
                break;
            case RacesEnum.Beorning:
                requirements.resources[ResourceType.WOOD] += prowess + defence;
                requirements.resources[ResourceType.LEATHER] += prowess + defence;
                break;
        }

        isLoaded = Initialize(CardClass.HazardCreature, requirements);
        return isLoaded;
    }
    public bool IsLoaded()
    {
        return isLoaded;
    }

    public string GetRaceStrings()
    {
        return GameObject.Find("Localization").GetComponent<Localization>().Localize(Enum.GetName(typeof(RacesEnum), race));
    }
        
    void Update()
    {
        if(!isLoaded)
            Initialize();
    }
    public List<string> GetAbilitiesStrings()
    {
        return hazardAbilities.Select(x => x.ToString()).ToList();
    }

    public StatusEffect GetStatusEffect()
    {
        if (hazardAbilities.Contains(HazardAbilitiesEnum.Curses))
            return StatusEffect.CURSES;
        else if (hazardAbilities.Contains(HazardAbilitiesEnum.Poisons))
            return StatusEffect.POISONS;
        else if (hazardAbilities.Contains(HazardAbilitiesEnum.Bleeding))
            return StatusEffect.BLEEDING;
        else
            return StatusEffect.NONE;
    }

    public int GetProwess()
    {
        return prowess;
    }

    public int GetDefence()
    {
        return defence;
    }
}
