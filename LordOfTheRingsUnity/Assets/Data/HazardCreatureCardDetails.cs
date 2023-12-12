using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HazardCreatureCardDetails: CardDetails
{
    [Header("HazardCreatureCardDetails")]
    public RacesEnum race;
    public SubRacesEnum subrace;

    public short prowess;
    public short defence;
        
    public List<HazardAbilitiesEnum> hazardAbilities;

    private bool isAwaken = false;
    private bool isLoaded;
    private List<CardTypesEnum> cardTypes;

    void Awake()
    {
        cardTypes = new();
        isLoaded = false;
        isAwaken = true;
    }

    public bool Initialize()
    {
        if (!isAwaken)
            Awake();
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
            case RacesEnum.OtherAnimals:
                requirements.resources[ResourceType.FOOD] += prowess + defence;
                requirements.resources[ResourceType.MOUNTS] += prowess + defence;
                break;
            case RacesEnum.Machinery:
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

        HashSet<CardTypesEnum> requiredCards = new();
        switch (race)
        {
            case RacesEnum.Man:
                switch (subrace)
                {
                    case SubRacesEnum.None:
                        requiredCards.Add(CardTypesEnum.FREE_BASTION);
                        break;
                    case SubRacesEnum.Easterling:
                    case SubRacesEnum.Dunlending:
                    case SubRacesEnum.Hillmen:
                    case SubRacesEnum.Haradrim:
                        requiredCards.Add(CardTypesEnum.NEUTRAL_BASTION);
                        break;
                    case SubRacesEnum.Marineers:
                        requiredCards.Add(CardTypesEnum.SEA);
                        break;
                    case SubRacesEnum.BlackNumenorean:
                        requiredCards.Add(CardTypesEnum.NEUTRAL_BASTION);
                        requiredCards.Add(CardTypesEnum.SEA);
                        break;
                    case SubRacesEnum.Woodmen:
                        requiredCards.Add(CardTypesEnum.FREE_BASTION);
                        break;
                    default:
                        requiredCards.Add(CardTypesEnum.FREE_BASTION);
                        break;
                }
                break;
            case RacesEnum.Beorning:
            case RacesEnum.Elf:
                requiredCards.Add(CardTypesEnum.FREE_BASTION);
                requiredCards.Add(CardTypesEnum.WILDERNESS);
                switch(subrace)
                {
                    case SubRacesEnum.Marineers:
                        requiredCards.Add(CardTypesEnum.SEA);
                        break;
                }
                break;
            case RacesEnum.Dwarf:
            case RacesEnum.Dunadan:
            case RacesEnum.Hobbit:
                requiredCards.Add(CardTypesEnum.FREE_BASTION);
                break;
            case RacesEnum.Ringwraith:
                requiredCards.Add(CardTypesEnum.DARK_BASTION);
                break;
            case RacesEnum.Orc:
                switch(subrace)
                {
                    case SubRacesEnum.None:
                        requiredCards.Add(CardTypesEnum.DARK_BASTION);
                        requiredCards.Add(CardTypesEnum.WILDERNESS);
                        break;
                    case SubRacesEnum.UrukHai:
                        requiredCards.Add(CardTypesEnum.DARK_BASTION);
                        break;
                    case SubRacesEnum.HalfOrc:
                        requiredCards.Add(CardTypesEnum.WILDERNESS);
                        break;
                    case SubRacesEnum.CaveGoblin:
                        requiredCards.Add(CardTypesEnum.LAIR);
                        requiredCards.Add(CardTypesEnum.WILDERNESS);
                        break;
                    default:
                        requiredCards.Add(CardTypesEnum.WILDERNESS);
                        break;
                }
                break;
            case RacesEnum.Troll:

                switch (subrace)
                {
                    case SubRacesEnum.OlogHai:
                        requiredCards.Add(CardTypesEnum.DARK_BASTION);
                        break;
                    case SubRacesEnum.WoodTroll:
                        requiredCards.Add(CardTypesEnum.WILDERNESS);
                        break;
                    case SubRacesEnum.CaveTroll:
                    case SubRacesEnum.StoneTroll:
                        requiredCards.Add(CardTypesEnum.LAIR);
                        requiredCards.Add(CardTypesEnum.WILDERNESS);
                        break;
                    default:
                        requiredCards.Add(CardTypesEnum.WILDERNESS);
                        break;
                }
                break;
            case RacesEnum.Wizard:
                requiredCards.Add(CardTypesEnum.FREE_BASTION);
                break;
            case RacesEnum.FallenWizard:
                requiredCards.Add(CardTypesEnum.NEUTRAL_BASTION);
                break;
            case RacesEnum.Balrog:
                requiredCards.Add(CardTypesEnum.DARK_BASTION);
                requiredCards.Add(CardTypesEnum.WILDERNESS);
                requiredCards.Add(CardTypesEnum.LAIR);
                break;
            case RacesEnum.Wolf:
            case RacesEnum.OtherAnimals:
            case RacesEnum.Machinery:
                requiredCards.Add(CardTypesEnum.WILDERNESS);
                break;
            case RacesEnum.Undead:
                requiredCards.Add(CardTypesEnum.LAIR);
                break;
            case RacesEnum.Spider:

                switch (subrace)
                {
                    case SubRacesEnum.LairSpider:
                        requiredCards.Add(CardTypesEnum.LAIR);
                        requiredCards.Add(CardTypesEnum.WILDERNESS);
                        break;
                    case SubRacesEnum.WoodSpider:
                        requiredCards.Add(CardTypesEnum.WILDERNESS);
                        break;
                    default:
                        requiredCards.Add(CardTypesEnum.WILDERNESS);
                        break;
                }
                break;
            case RacesEnum.Plant:
                requiredCards.Add(CardTypesEnum.WILDERNESS);
                break;
            case RacesEnum.Bear:
                requiredCards.Add(CardTypesEnum.WILDERNESS);
                break;
            case RacesEnum.Dragon:
                requiredCards.Add(CardTypesEnum.LAIR);
                requiredCards.Add(CardTypesEnum.WILDERNESS);
                break;
            case RacesEnum.Maia:
                requiredCards.Add(CardTypesEnum.WILDERNESS);
                break;
            case RacesEnum.Weather:
                requiredCards.Add(CardTypesEnum.WILDERNESS);
                break;
            case RacesEnum.Giant:
                requiredCards.Add(CardTypesEnum.WILDERNESS);
                requiredCards.Add(CardTypesEnum.LAIR);
                break;
        }

        int totalCards = (int) Math.Ceiling((prowess + defence + hazardAbilities.Count() * 3) / 3f);
        for(int i=0;i<totalCards;i++)
            cardTypes.Add(requiredCards.ToList()[UnityEngine.Random.Range(0, requiredCards.Count())]);

        isLoaded = Initialize(CardClass.HazardCreature, requirements);
        return isLoaded;
    }

    public List<CardTypesEnum> GetCardTypes()
    {
        return cardTypes;
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

    public List<HazardAbilitiesEnum> GetAbilities()
    {
        return hazardAbilities;
    }

    public StatusEffect GetStatusEffect()
    {
        if (hazardAbilities.Contains(HazardAbilitiesEnum.Curses))
            return StatusEffect.MORGUL;
        else if (hazardAbilities.Contains(HazardAbilitiesEnum.Poisons))
            return StatusEffect.POISON;
        else if (hazardAbilities.Contains(HazardAbilitiesEnum.Bleeding))
            return StatusEffect.BLOOD;
        else if (hazardAbilities.Contains(HazardAbilitiesEnum.Blinds))
            return StatusEffect.BLIND;
        else if (hazardAbilities.Contains(HazardAbilitiesEnum.Freezes))
            return StatusEffect.ICE;
        else if (hazardAbilities.Contains(HazardAbilitiesEnum.Traps))
            return StatusEffect.IMMOVIBILITY;
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
