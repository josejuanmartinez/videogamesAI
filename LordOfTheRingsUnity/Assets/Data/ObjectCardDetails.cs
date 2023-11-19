using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectCardDetails : CardDetails
{
    [Header("Object Card Details")]
    public ObjectType objectSlot;
    public List<ResourceType> materials;
    public List<CharacterClassEnum> requiredClass;
    public List<ObjectAbilities> abilities;

    public short prowess;
    public short defence;
    public short mind;
    public short influence;
    public short movement;

    void Awake()
    {
        Resources requirements = new(0, 0, 0, 0, 0, 0, 0, 0);

        int units = requirements.resources[ResourceType.METAL] += prowess + defence + Math.Abs(mind) + influence + movement + abilities.Count;

        requirements.resources[ResourceType.GOLD] += 2*units;
        foreach(ResourceType material in materials)
        {
            switch (material)
            {
                case ResourceType.FOOD:
                    requirements.resources[ResourceType.FOOD] += 5 * units;
                    break;
                case ResourceType.GOLD:
                    requirements.resources[ResourceType.GOLD] += 2 * units;
                    break;
                case ResourceType.CLOTHES:
                    requirements.resources[ResourceType.CLOTHES] += 2 * units;
                    break;
                case ResourceType.WOOD:
                    requirements.resources[ResourceType.WOOD] += 2 * units;
                    break;
                case ResourceType.METAL:
                    requirements.resources[ResourceType.METAL] += 2 * units;
                    break;
                case ResourceType.MOUNTS:
                    requirements.resources[ResourceType.MOUNTS] += 2 * units;
                    break;
                case ResourceType.GEMS:
                    requirements.resources[ResourceType.GEMS] += 2 * units;
                    break;
                case ResourceType.LEATHER:
                    requirements.resources[ResourceType.LEATHER] += 2 * units;
                    break;
            }
        }
        
        base.Initialize(CardClass.Object, requirements);
    }
    public void Initialize()
    {
        Awake();
    }
    public bool IsBattleItem()
    {
        return objectSlot == ObjectType.OtherHand || objectSlot == ObjectType.MainHand || objectSlot == ObjectType.Armor || objectSlot == ObjectType.Head;
    }
    public List<string> GetClassesStrings()
    {
        return requiredClass.Select(x=> x.ToString()).ToList();
    }
    public List<string> GetAbilitiesString()
    {
        return abilities.Select(x => x.ToString()).ToList();
    }
}
