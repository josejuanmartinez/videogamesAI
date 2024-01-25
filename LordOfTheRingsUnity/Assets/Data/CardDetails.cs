using System.IO;
using System;
using UnityEngine;
using System.Security.Cryptography;

public class CardDetails : MonoBehaviour
{
    [Header("Card Details")]
    [PreviewSprite]
    public Sprite cardSprite;
    public string cardId;
    public string descForAIGeneration;
    public CardClass cardClass;
    public bool isUnique;
    public string hometown;

    /* AUTOMATICALLY GENERATED */
    [SerializeField]
    private short victoryPoints;
    [SerializeField]
    private short corruption;
    [SerializeField]
    private Resources resourcesRequired;

    private Game game;
    private bool isInitialized;

    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        isInitialized = false;
    }

    protected bool Initialize(CardClass cardClass, Resources resourcesRequired)
    {
        Awake();

        //if (!game.IsInitialized())
        //    return false;

        this.resourcesRequired = resourcesRequired;
        this.cardClass = cardClass;

        switch (cardClass)
        {
            case CardClass.Place:
                break;
            case CardClass.Character:
                resourcesRequired.resources[ResourceType.GOLD] += 25;
                break;
            case CardClass.Object:
                resourcesRequired.resources[ResourceType.GOLD] += 15;
                break;
            case CardClass.Faction:
                resourcesRequired.resources[ResourceType.GOLD] += 25;
                break;
            case CardClass.Event:
                resourcesRequired.resources[ResourceType.GOLD] += 25;
                break;
            case CardClass.HazardEvent:
                resourcesRequired.resources[ResourceType.GOLD] += 25;
                break;
            case CardClass.HazardCreature:
                resourcesRequired.resources[ResourceType.GOLD] += 25;
                break;
            case CardClass.Ally:
                resourcesRequired.resources[ResourceType.GOLD] += 25;
                break;
            case CardClass.GoldRing:
                resourcesRequired.resources[ResourceType.GOLD] += 50;
                break;
            case CardClass.Ring:
                resourcesRequired.resources[ResourceType.GOLD] += 25;
                break;
            case CardClass.NONE:
                break;
        }
        CalculateCorruption();
        CalculateVictoryPoints();
        isInitialized = true;
        return isInitialized;
    }

    public Resources GetResourcesRequired()
    {
        return resourcesRequired;
    }

    protected bool IsInitialized()
    {
        return isInitialized;
    }

    public bool IsClassOf(CardClass cardClass)
    {
        return cardClass switch
        {
            CardClass.Object => this is ObjectCardDetails,
            CardClass.HazardCreature => this is HazardCreatureCardDetails,
            CardClass.Character => this is CharacterCardDetails,
            CardClass.Ally => this is AllyCardDetails,
            CardClass.Event => this is EventCardDetails,
            CardClass.GoldRing => this is GoldRingDetails,
            CardClass.Ring => this is RingCardDetails,
            CardClass.Place => false,
            CardClass.Faction => this is FactionCardDetails,
            CardClass.HazardEvent => this is HazardEventCardDetails,
            CardClass.NONE => false,
            _ => false,
        };
    }

    private void CalculateCorruption()
    {
        int res = 0;
        switch (cardClass)
        {
            case CardClass.Place:
                res = 0;
                break;
            case CardClass.Character:
                res = 0;
                break;
            case CardClass.Object:
                ObjectCardDetails objectDetails = this as ObjectCardDetails;
                if(objectDetails != null)
                {
                    switch (objectDetails.objectSlot)
                    {
                        case ObjectType.Consumable:
                            res = 0;
                            break;
                        case ObjectType.MainHand:
                        case ObjectType.OtherHand:
                        case ObjectType.Head:
                        case ObjectType.Gloves:
                        case ObjectType.Cloak:
                        case ObjectType.Belt:
                        case ObjectType.Boots:
                        case ObjectType.Mount:
                            res = 1;
                            break;
                        case ObjectType.Armor:
                            res = 2;
                            break;
                        case ObjectType.Jewelry:
                            res = 3;
                            break;
                        case ObjectType.Palantir:
                            res = 4;
                            break;
                    }
                }
                break;
            case CardClass.Faction:
                res = 0;
                break;
            case CardClass.Event:
                res = 0;
                break;
            case CardClass.HazardEvent:
                res = 0;
                break;
            case CardClass.HazardCreature:
                res = 0;
                break;
            case CardClass.Ally:
                res = 0;
                break;
            case CardClass.GoldRing:
                res = 4;
                break;
            case CardClass.Ring:
                RingCardDetails ringDetails = this as RingCardDetails;
                if (ringDetails != null)
                {
                    switch (ringDetails.objectSlot)
                    {
                        case RingType.MindRing:
                        case RingType.DwarvenRing:
                        case RingType.MagicRing:
                            res = 2;
                            break;
                        case RingType.LesserRing:
                            res = 1;
                            break;
                        case RingType.TheOneRing:
                            res = 5;
                            break;
                        case RingType.Unknown:
                            res = 2;
                            break;
                    }
                }
                break;
            case CardClass.NONE:
                res = 0;
                break;
        }

        corruption = (short) res;
    }

    public int GetCorruption()
    {
        return corruption + game.GetCorruptionBaseByDifficulty();
    }

    private void CalculateVictoryPoints()
    {
        int res = 0;
        switch (cardClass)
        {
            case CardClass.Place:
                res = 0;
                break;
            case CardClass.Character:
                CharacterCardDetails characterDetails = this as CharacterCardDetails;
                res = characterDetails.GetMind() +
                    characterDetails.GetInfluence() +
                    characterDetails.GetProwess() +
                    characterDetails.GetDefence() +
                    characterDetails.GetAbilities().Count;
                res = (int) Math.Ceiling((decimal) res / 5);
                break;
            case CardClass.Object:
                ObjectCardDetails objectDetails = this as ObjectCardDetails;
                if (objectDetails != null)
                {
                    switch (objectDetails.objectSlot)
                    {
                        case ObjectType.Consumable:
                            res = 0;
                            break;
                        case ObjectType.MainHand:
                        case ObjectType.OtherHand:
                            res = 1;
                            break;
                        case ObjectType.Head:
                        case ObjectType.Gloves:
                            res = 0;
                            break;
                        case ObjectType.Cloak:
                            res = 1;
                            break;
                        case ObjectType.Belt:
                        case ObjectType.Boots:
                        case ObjectType.Mount:
                            res = 0;
                            break;
                        case ObjectType.Armor:
                            res = 2;
                            break;
                        case ObjectType.Jewelry:
                            res = 3;
                            break;
                        case ObjectType.Palantir:
                            res = 4;
                            break;
                    }
                }
                break;
            case CardClass.Faction:
                res = 2;
                break;
            case CardClass.Event:
                res = 0;
                break;
            case CardClass.HazardEvent:
                res = 0;
                break;
            case CardClass.HazardCreature:
                HazardCreatureCardDetails creatureDetails = this as HazardCreatureCardDetails;
                res = creatureDetails.GetProwess() + creatureDetails.GetDefence() + creatureDetails.GetAbilities().Count;
                res = (int)Math.Ceiling((decimal)res / 3);
                break;
            case CardClass.Ally:
                res = 1;
                break;
            case CardClass.GoldRing:
                res = 4;
                break;
            case CardClass.Ring:
                RingCardDetails ringDetails = this as RingCardDetails;
                if (ringDetails != null)
                {
                    switch (ringDetails.objectSlot)
                    {
                        case RingType.MindRing:
                        case RingType.DwarvenRing:
                        case RingType.MagicRing:
                            res = 2;
                            break;
                        case RingType.LesserRing:
                            res = 1;
                            break;
                        case RingType.TheOneRing:
                            res = 5;
                            break;
                        case RingType.Unknown:
                            res = 2;
                            break;
                    }
                }
                break;
            case CardClass.NONE:
                res = 0;
                break;
        }

        victoryPoints = (short) res;
    }

    public int GetVictoryPoints()
    {
        int vp = victoryPoints - game.GetVictoryPointsBaseByDifficulty();
        if (victoryPoints > 0)
            vp = Math.Max(1, vp);
        return vp;
    }


    public bool IsMovableClass()
    {
        return cardClass == CardClass.Character || cardClass == CardClass.HazardCreature;
    }

    public CharAtCityRequiredEnum IsCharAtCityRequired()
    {
        switch (cardClass)
        {
            case CardClass.Place:
                return CharAtCityRequiredEnum.NONE;
            case CardClass.Character:
                return CharAtCityRequiredEnum.NONE;
            case CardClass.Object:
                return CharAtCityRequiredEnum.FOREIGNCITY;
            case CardClass.Faction:
                return CharAtCityRequiredEnum.SPECIFICCITY;
            case CardClass.Event:
                return !string.IsNullOrEmpty(hometown) ? CharAtCityRequiredEnum.SPECIFICCITY : CharAtCityRequiredEnum.NONE;
            case CardClass.HazardEvent:
                return !string.IsNullOrEmpty(hometown) ? CharAtCityRequiredEnum.SPECIFICCITY : CharAtCityRequiredEnum.NONE;
            case CardClass.HazardCreature:
                return CharAtCityRequiredEnum.NONE;
            case CardClass.Ally:
                return CharAtCityRequiredEnum.SPECIFICCITY;
            case CardClass.GoldRing:
                return CharAtCityRequiredEnum.FOREIGNCITY;
            case CardClass.Ring:
                return CharAtCityRequiredEnum.ONLYCHAR;
            case CardClass.NONE:
            default:
                return CharAtCityRequiredEnum.NONE;
        }
    }

    public  string GetHomeTown()
    {
        return hometown;
    }
}