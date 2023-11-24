using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RingCardDetails : CardDetails
{
    [Header("Ring Card Details")]
    public RingType objectSlot;
    public List<CharacterClassEnum> requiredClass;
    public List<ObjectAbilities> abilities;

    public short prowess;
    public short defence;
    public short mind;
    public short influence;

    private bool isLoaded;

    void Awake()
    {
        isLoaded = false;    
    }

    public bool Initialize()
    {
        Resources requirements = new (0, 0, 0, 0, 0, 0, 0, 0);
        int units = prowess + defence + Math.Abs(mind) + influence;
        switch (objectSlot)
        {
            case RingType.MindRing:
                requirements.resources[ResourceType.GOLD] += units * 3;
                break;
            case RingType.DwarvenRing:
                requirements.resources[ResourceType.METAL] += units * 3;
                break;
            case RingType.MagicRing:
                requirements.resources[ResourceType.GEMS] += units * 3;
                break;
            case RingType.LesserRing:
                requirements.resources[ResourceType.METAL] += units * 2;
                break;
            case RingType.TheOneRing:
                requirements.resources[ResourceType.GOLD] += units * 10;
                break;
            case RingType.Unknown:
                break;
        }
        isLoaded = Initialize(CardClass.Ring, requirements);
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
    public string GetSlotString()
    {
        return GameObject.Find("Localization").GetComponent<Localization>().LocalizeWithSprite(Enum.GetName(typeof(RingType), objectSlot));
    }
    public List<string> GetClassesStrings()
    {
        return requiredClass.Select(x =>  x.ToString()).ToList();
    }

    public List<string> GetAbilitiesStrings()
    {
        return abilities.Select(x => x.ToString()).ToList();
    }
}